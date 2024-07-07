#include <stdint.h>
#include "instruction.h"

#include <stdio.h>
#include <stdlib.h>

OPCODE decode_opcode(const uint8_t instruction) {
	switch (instruction) {
		case 0:  return ADD;
		case 1:  return ADDI;
		case 2:  return OR;
		case 3:  return XOR;
		case 4:  return SHLL;
		case 5:  return SHLR;
		case 6:  return NEG;
		case 7:  return AND;
		case 8:  return LW;
		case 9:  return SW;
		case 10: return BRC;
		case 11: return JALR;
		case 12: return PUSH;
		case 13: return POP;
		case 14: return LUI;
		case 15: return SIRA;
		case 17: return ORI;
		case 18: return CSRW;
		case 19: return CSRR;
		default: return INVALID;
	}
}

char* get_instruction_name(const uint8_t opcode) {
	switch (opcode) {
		case ADD: return "add";
		case ADDI: return "addi";
		case OR: return "or";
		case XOR: return "xor";
		case SHLL: return "shll";
		case SHLR: return "shlr";
		case NEG: return "neg";
		case AND: return "and";
		case LW: return "lw";
		case SW: return "sw";
		case BRC: return "brc";
		case JALR: return "jalr";
		case PUSH: return "push";
		case POP: return "pop";
		case LUI: return "lui";
		case SIRA: return "sira";
		case ORI: return "ori";
		case CSRW: return "csrw";
		case CSRR: return "csrr";
		default: return "INVALID";
	}
}