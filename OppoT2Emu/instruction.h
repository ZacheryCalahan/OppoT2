#pragma once

typedef struct INSTRUCTION_DATA {
	Instruction opcode;
	uint8_t regA;
	uint8_t regB;
	uint8_t regC;
	uint8_t conditional;
	uint32_t immediate;
} Instruction_Data;

typedef enum INSTRUCTION_FORMAT {
	RRR,
	RRI,
	RI,
	RRCI,
	R,
	OP,
	RR
} INSTRUCTION_FORMAT;

typedef enum INSTRUCTIONS {
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
	ORI,
	CSRW,
	CSRR
} Instruction;

Instruction_Data decode_instruction(uint32_t instruction);