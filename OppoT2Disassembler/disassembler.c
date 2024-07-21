#include "headers/disassembler.h"

#include <stdint.h>
#include <stdio.h>
#include <stdlib.h>
#include <string.h>

#include "instruction.h"
#include "memory.h"

char* disassembleInstruction(const uint32_t instruction) {
    // Get data on the instruction

    const uint8_t opcode = instruction >> 27;
    const uint8_t regA = (instruction >> 22) & 0b11111;
    const uint8_t regB = (instruction >> 17) & 0b11111;
    const uint8_t regC = instruction & 0b11111;
    uint32_t immediate = instruction & 0x1FFFF;
    const uint8_t cond = (instruction >> 14) & 0b111;

    char* instructionString = malloc(32);

    if (instruction == 0) {
        return strdup("");
    }

    switch(decode_opcode(opcode)) {
        case ADD:
            sprintf(instructionString, "add %s, %s, %s",
                get_register_name(regA),
                get_register_name(regB),
                get_register_name(regC));
            break;

        case ADDI:
            if (immediate >> 16 == 1) {
                immediate |= 0b11111111111111100000000000000000; // Convert to signed 17bit
            }
            sprintf(instructionString, "addi %s, %s, %d",
                get_register_name(regA),
                get_register_name(regB),
                immediate);
            break;

        case OR:
            sprintf(instructionString, "or %s, %s, %s",
                get_register_name(regA),
                get_register_name(regB),
                get_register_name(regC));
            break;

        case XOR:
            sprintf(instructionString, "xor %s, %s, %s",
                get_register_name(regA),
                get_register_name(regB),
                get_register_name(regC));
            break;

        case SHLL:
            sprintf(instructionString, "shll %s, %s, %s",
                get_register_name(regA),
                get_register_name(regB),
                get_register_name(regC));
            break;

        case SHLR:
            sprintf(instructionString, "shll %s, %s, %s",
                get_register_name(regA),
                get_register_name(regB),
                get_register_name(regC));
            break;

        case NEG:
            sprintf(instructionString, "neg %s, %s",
                get_register_name(regA),
                get_register_name(regB));
            break;

        case AND:
            sprintf(instructionString, "and %s, %s, %s",
                get_register_name(regA),
                get_register_name(regB),
                get_register_name(regC));
            break;

        case LW:
            if (immediate >> 16 == 1) {
                immediate |= 0b11111111111111100000000000000000; // Convert to signed 17bit
            }
            sprintf(instructionString, "lw %s, %s, %d",
                get_register_name(regA),
                get_register_name(regB),
                immediate);
            break;

        case SW:
            if (immediate >> 16 == 1) {
                immediate |= 0b11111111111111100000000000000000; // Convert to signed 17bit
            }
            sprintf(instructionString, "sw %s, %s, %d",
                get_register_name(regA),
                get_register_name(regB),
                immediate);
            break;

        case BRC:
            immediate &= 0b00000000000000000001111111111111;
            if (immediate >> 12 == 1) {
                immediate |= 0b11111111111111111110000000000000; // Convert to signed 13bit
            }

            sprintf(instructionString, "brc %s, %s, %s, %d",
                get_register_name(regA),
                get_register_name(regB),
                get_cond_name(cond),
                immediate);
            break;

        case JALR:
            sprintf(instructionString, "jalr %s, %s",
                get_register_name(regA),
                get_register_name(regB));
            break;

        case PUSH:
            sprintf(instructionString, "push %s",
                get_register_name(regA));
            break;

        case POP:
            sprintf(instructionString, "pop %s",
                get_register_name(regA));
            break;

        case LUI:
            immediate &= 0b111111111111111; // 15 bit
            immediate = immediate << 17;
            sprintf(instructionString, "lui %s, %d",
                get_register_name(regA),
                immediate);
            break;

        case SIRA:
            sprintf(instructionString, "sira %s",
                get_register_name(regA));
            break;

        case INTR:
            sprintf(instructionString, "intr %s",
                get_register_name(regA));
            break;

        case ORI:
            immediate &= 0b11111111111111111; // 17 bit
            sprintf(instructionString, "ori %s, %d",
                get_register_name(regA),
                immediate);
            break;

        case CSRW:
            sprintf(instructionString, "csrw %s",
                get_register_name(regA));
            break;

        case CSRR:
            sprintf(instructionString, "csrr %s",
                get_register_name(regA));
            break;

        default:
            sprintf(instructionString, "INVALID");
            break;
    }

    char* str = strdup(instructionString);
    free(instructionString);
    return str;
}

extern Memory mem;

void generate_file(const char* file_path) {
    FILE* file = fopen(file_path, "w");

    // Traverse through each page (This is not the best solution, but it's good enough for how any given program is coded.)
    for (int i = 0; i < NUM_PAGES; i++) {

        // Check to see if the page is null, otherwise skip it.
        if (mem.pages[i] != NULL) {
            // Get entries in this page
            for (int j = 0; j < PAGE_SIZE; j++) {
                const uint32_t address = page_to_address(i, j);

                char* line = malloc(45);
                char* str = disassembleInstruction(readRAM(address));
                if (strcmp("", str) != 0) {
                    sprintf(line, "%08X : %s\n", address, str);
                    fputs(line, file);
                }

                free(str);
                free(line);

            }
        }
    }

    fclose(file);
}