#include <interface.h>
#include <stdio.h>
#include <stdlib.h>
#include <windows.h>
#include <stdint.h>

#include "emulator.h"
#include "instruction.h"
#include "main.h"
#include "memmap.h"
#include "memory.h"

void interrupt_routine(void);

// All CPU state variables
struct CPU_STATE CPU_STATE;
boolean int_flag = FALSE;
uint32_t int_status, int_data;
char key_buffer;

// Set all portions of the CPU state to zero.
void init() {
    CPU_STATE.isrSaveReg = 0;
    CPU_STATE.pc = 0;
    CPU_STATE.csr = 0;

    for (int i = 0; i < 32; i++) {
        CPU_STATE.regFile[i] = 0;
    }

    initialize_memory();
}

// Read a file to memory
void read_bin_to_memory(FILE *fptr) {
    // Open BIN file and write to RAM

    if (fptr == NULL) {
        printf("Error opening file!\n");
        exit(1);
    }

    // Get length
    fseek(fptr, 0, SEEK_END);
    const long len = ftell(fptr);
    rewind(fptr);
    uint32_t address = 0;

    // Read data
    for (int i = 0; i < len; i++) {
        uint32_t data = fgetc(fptr) << 24;
        data |= fgetc(fptr) << 16;
        data |= fgetc(fptr) << 8;
        data |= fgetc(fptr);
        writeRAM(address, data);
        address++;
    }

    fclose(fptr);
}

// Emulate the clock cycle of the CPU.
void emulate_cycle() {
    if (DEBUG) {
        printf("Instruction: %s", get_instruction_name((uint8_t) (readRAM(CPU_STATE.pc) >> 27)));
        printf(" PC: %08X\n\n", CPU_STATE.pc);
    }

    const uint32_t instruction = readRAM(CPU_STATE.pc);

    const uint8_t regA = (instruction >> 22) & 0b11111;
    const uint8_t regB = (instruction >> 17) & 0b11111;
    const uint8_t regC = instruction & 0b11111;
    uint32_t immediate = instruction & 0x1FFFF;
    const uint8_t cond = (instruction >> 14) & 0b111;

    // Decode the instruction and execute
    switch (decode_opcode(instruction >> 27)) {
        case ADD:
            CPU_STATE.regFile[regA] = CPU_STATE.regFile[regB] + CPU_STATE.regFile[regC];
            break;
        case ADDI:
            if (immediate >> 16 == 1) {
                immediate |= 0b11111111111111100000000000000000; // Convert to signed 17bit
            }
            CPU_STATE.regFile[regA] = CPU_STATE.regFile[regB] + immediate;
            break;
        case OR:
            CPU_STATE.regFile[regA] = CPU_STATE.regFile[regB] | CPU_STATE.regFile[regC];
            break;
        case XOR:
            CPU_STATE.regFile[regA] = CPU_STATE.regFile[regB] ^ CPU_STATE.regFile[regC];
            break;
        case SHLL:
            CPU_STATE.regFile[regA] = CPU_STATE.regFile[regB] << (CPU_STATE.regFile[regC] & 0b11111);
            break;
        case SHLR:
            CPU_STATE.regFile[regA] = CPU_STATE.regFile[regB] >> (CPU_STATE.regFile[regC] & 0b11111);
            break;
        case NEG:
            CPU_STATE.regFile[regA] = ~CPU_STATE.regFile[regB];
            break;
        case AND:
            CPU_STATE.regFile[regA] = CPU_STATE.regFile[regB] & CPU_STATE.regFile[regC];
            break;
        case LW:
            if (immediate >> 16 == 1) {
                immediate |= 0b11111111111111100000000000000000; // Convert to signed 17bit
            }

            CPU_STATE.regFile[regA] = readMem(CPU_STATE.regFile[regB] + immediate);
            break;
        case SW:
            if (immediate >> 16 == 1) {
                immediate |= 0b11111111111111100000000000000000; // Convert to signed 17bit
            }
            writeMem(CPU_STATE.regFile[regB] + immediate, CPU_STATE.regFile[regA]);
            break;
        case BRC:
            immediate &= 0b00000000000000000001111111111111;
            if (immediate >> 12 == 1) {
                immediate |= 0b11111111111111111110000000000000; // Convert to signed 13bit
            }
            // Here the conditional should be handled.

            switch (cond) {
                case 0:
                    if (CPU_STATE.regFile[regA] == CPU_STATE.regFile[regB]) {
                        CPU_STATE.pc = CPU_STATE.pc + immediate;
                    }
                break;
                case 1:
                    if (CPU_STATE.regFile[regA] != CPU_STATE.regFile[regB]) {
                        CPU_STATE.pc = CPU_STATE.pc + immediate;
                    }
                break;
                case 2:
                    if (CPU_STATE.regFile[regA] > CPU_STATE.regFile[regB]) {
                        CPU_STATE.pc = CPU_STATE.pc + immediate;
                    }
                break;
                case 3:
                    if (CPU_STATE.regFile[regA] >= CPU_STATE.regFile[regB]) {
                        CPU_STATE.pc = CPU_STATE.pc + immediate;
                    }
                break;
                case 4:
                    if (CPU_STATE.regFile[regA] < CPU_STATE.regFile[regB]) {
                        CPU_STATE.pc = CPU_STATE.pc + immediate;
                    }
                break;
                case 5:
                    if (CPU_STATE.regFile[regA] <= CPU_STATE.regFile[regB]) {
                        CPU_STATE.pc = CPU_STATE.pc + immediate;
                    }
                break;
                case 6:
                    CPU_STATE.pc = CPU_STATE.pc + immediate;
                break;
                default:
                    break;
            }
            break;

        case JALR:
            CPU_STATE.regFile[regA] = CPU_STATE.pc + 1;
            CPU_STATE.pc = CPU_STATE.regFile[regB] - 1;
            break;

        case PUSH:
            // It is odd that the stack pointer is pushed to sp - 1, but that's what works I guess.
            writeRAM(CPU_STATE.regFile[31] - 1, CPU_STATE.regFile[regA]);
            CPU_STATE.regFile[31]--;
            break;

        case POP:
            CPU_STATE.regFile[regA] = readRAM(CPU_STATE.regFile[31]);
            CPU_STATE.regFile[31]++;
            break;

        case LUI:
            immediate &= 0b111111111111111; // 15 bit
            immediate = immediate << 17;
            CPU_STATE.regFile[regA] = immediate;
            break;

        case SIRA:
            CPU_STATE.regFile[regA] = CPU_STATE.isrSaveReg;
            break;

        case ORI:
            immediate &= 0b11111111111111111; // 17 bit
            CPU_STATE.regFile[regA] = CPU_STATE.regFile[regB] | immediate;
            break;

        case CSRW:
            CPU_STATE.csr = CPU_STATE.regFile[regA];
            break;

        case CSRR:
            CPU_STATE.regFile[regA] = CPU_STATE.csr;
            break;

        default:
            break;
    }

    // Set state for next cycle
    CPU_STATE.pc += 1;
    CPU_STATE.regFile[0] = 0; // This is the easiest way to ensure r0 always equals zero.

    if (CPU_STATE.irq) {
        interrupt_routine();
        CPU_STATE.irq = 0;
    }
    int_flag = FALSE;
}

void interrupt_routine() {
    // Check to see if INTS are disabled
    if ((CPU_STATE.csr & 0x80000000) == 0x80000000) {
        return;
    }

    // 1. Save PC state (always ran after instruction is complete, so just save the PC)
    CPU_STATE.isrSaveReg = CPU_STATE.pc;

    // 2. Get the controller that requested the interrupt and pull its data
    const uint32_t lowest_bit = CPU_STATE.interrupt_source_register & -CPU_STATE.interrupt_source_register;
    uint32_t controller_data;

    // Emulate a priority encoder, and take the lowest bit as most important.
    switch (lowest_bit) {
        case (PERIPHERAL_CONTROL_MASK):
            controller_data = PERIPHERAL_CONTROL_ROUTINE;
            break;
        case (STORAGE_CONTROL_MASK):
            controller_data = STORAGE_CONTROL_ROUTINE;
            break;
        default:
            printf("IRQ Error. IntSource: %d\n", lowest_bit);
            exit(1);
    }

    // 3. Set the ISTAT to the data in the int source register
    CPU_STATE.regFile[30] = CPU_STATE.interrupt_source_register;

    // 3. Jump to the Interrupt Service Routine location.
    CPU_STATE.pc = 0x2000;
}

void writeMem(const uint32_t address, const uint32_t data) {
    // Check against mapped I/O ports

    // TTY
    if (address == TTY_OUTPUT_POINTER) {
        write_to_tty(data);
        return;
    }

    writeRAM(address, data);
}

uint32_t readMem(const uint32_t address) {

    if (address == 0xFFFFFFFE) {
        // Get the char in the keyboard buffer
        return key_buffer;
    }

    return readRAM(address);
}

// Print the current state of the CPU
void print_state() {
    printf("PC = %x\n", CPU_STATE.pc - 1);
    printf("isrSaveReg = %x\n", CPU_STATE.isrSaveReg);
    for (int i = 0; i < 32; i++) {
        printf("regFile[%d] = %x\n", i, CPU_STATE.regFile[i]);
    }
}

