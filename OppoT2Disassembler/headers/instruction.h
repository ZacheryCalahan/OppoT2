#pragma once

typedef enum {
    RRR,
    RRI,
    RI,
    RRCI,
    R,
    OP,
    RR,
    INVALID
} INSTRUCTION_FORMAT;

typedef enum {
    ADD,
    ADDI,
    OR,
    XOR,
    SHLL,
    SHLR,
    NEG,
    AND,
    LW,
    SW,
    BRC,
    JALR,
    PUSH,
    POP,
    LUI,
    SIRA,
    INTR,
    ORI,
    CSRW,
    CSRR
} OPCODE;

typedef enum {
    EQ,
    NEQ,
    GT,
    GTE,
    LT,
    LTE,
    PASS,
    UNDEFINED
} COND;



OPCODE decode_opcode(uint8_t instruction);
char* get_instruction_name(uint8_t opcode);
char* get_register_name(uint8_t register_int);
char* get_cond_name(COND cond);