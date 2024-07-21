#include <stdint.h>
#include "instruction.h"

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

char* get_register_name(const uint8_t register_int) {
    switch(register_int) {
        case 0: return "r0";
        case 1: return "ra";
        case 2: return "s0";
        case 3: return "s1";
        case 4: return "s2";
        case 5: return "s3";
        case 6: return "s4";
        case 7: return "s5";
        case 8: return "s6";
        case 9: return "s7";
        case 10: return "t0";
        case 11: return "t1";
        case 12: return "t2";
        case 13: return "t3";
        case 14: return "t4";
        case 15: return "t5";
        case 16: return "t6";
        case 17: return "t7";
        case 18: return "r18";
        case 19: return "r19";
        case 20: return "r20";
        case 21: return "r21";
        case 22: return "r22";
        case 23: return "r23";
        case 24: return "r24";
        case 25: return "r25";
        case 26: return "r26";
        case 27: return "r27";
        case 28: return "r28";
        case 29: return "r29";
        case 30: return "isr";
        case 31: return "r31";
        default: return "INVALID";
    }
}

char* get_cond_name(const COND cond) {
    switch(cond) {
        case EQ: return "eq";
        case NEQ: return "neq";
        case GT: return "gt";
        case GTE: return "gte";
        case LT: return "lt";
        case LTE: return "lte";
        case PASS: return "pass";
        default: return "ERROR";
    }
}