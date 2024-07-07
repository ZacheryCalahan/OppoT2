#include <stdint.h>
#include <stdio.h>
#include <stdlib.h>
#include <windows.h>

#include "memory.h"
#include "instruction.h"

#define CLK_SPEED 0
// Private Functions

void init(void);
void emulate_cycle(uint32_t instruction);
void print_state(void);
void read_bin_to_memory(const char* file);
void handle_interrupts(void);
void writeMem(uint32_t address, uint32_t data);
uint32_t readMem(uint32_t address);

// Global Variables
struct CPU_STATE {
    // Program Counter
    uint32_t pc;
    uint32_t isrSaveReg;

    // Registers
    uint32_t regFile[32];
    uint32_t csr;

} CPU_STATE;

int main(int argc, char* argv[]) {
    init();

    read_bin_to_memory(argv[1]);
    // ReSharper disable once CppDFAEndlessLoop
    while(1) {
        emulate_cycle(readRAM(CPU_STATE.pc));
        handle_interrupts();
    }
}

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
void read_bin_to_memory(const char* file) {
    // Open BIN file and write to RAM
    FILE *fptr = fopen(file, "r");

    if (fptr == NULL) {
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

    // for (int i = 0; i < len; i++) {
    //     printf("Address: 0x%08X Data:0x%08X\n", i, readRAM(i));
    // }
    fclose(fptr);
}

// Emulate the clock cycle of the CPU.
void emulate_cycle(const uint32_t instruction) {
    // Emulate timed clk cycle
    Sleep(CLK_SPEED);
    //printf("PC: %08X Instruction is %s\n",CPU_STATE.pc, get_instruction_name(instruction >> 27));

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
            CPU_STATE.pc = CPU_STATE.regFile[regB] - 1;
            CPU_STATE.regFile[regA] = CPU_STATE.pc + 1;
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
}

// Handle interrupts
void handle_interrupts() {

}

void writeMem(const uint32_t address, const uint32_t data) {
    // Check against mapped I/O ports

    // TTY
    if (address == 0xFFFFFFFF) {
        printf("%c", (char) data);
        return;
    }

    writeRAM(address, data);
}

uint32_t readMem(const uint32_t address) {

    if (address == 0xFFFFFFFE) {
        // Do things here.
        return 0;
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