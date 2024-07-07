#include <stdint.h>

#include "instruction.h"

Instruction_Data decode_instruction(uint32_t instruction) {
	Instruction_Data instruction_data; // The container being build
	Instruction instruction_type; // The opcode
	INSTRUCTION_FORMAT instruction_format = decode_opcode(instruction, &instruction_type); // The type of instruction

	// Build the instruction data incrementally, stopping if an instruction is finished due to its type.

	// Handle the opcodes
	instruction_data.opcode = instruction_type;
	if (instruction_type == OP) {
		return instruction_data;
	}

	// Handle rA
	uint8_t rA = (instruction >> 22) & 0b11111;
	instruction_data.regA = rA;
	if (instruction_type == R) {
		return instruction_data;
	}

	// Handle RI types
	if (instruction_type == RI) {
		uint32_t immediate = instruction & 0x1FFFF; // 17bit immediate
		instruction_data.immediate = immediate;
		return instruction_data;
	}

	// Handle rB
	uint8_t rB = (instruction >> 17) & 0b11111;
	instruction_data.regB = rB;
	if (instruction_type == RR) {
		return instruction_data;
	}

	// Remaining instruction formats are not identical beyond what's already been computed.

	// Handle RRR cases
	if (instruction_type == RRR) {
		uint8_t rC = instruction & 0b11111;
		instruction_data.regC = rC;
		return instruction_data;
	}

	// Handle RRI cases
	if (instruction_type == RRI) {
		uint32_t immediate = instruction & 0x1FFFF; // 17bit immediate
		instruction_data.immediate = immediate;
		return instruction_data;
	}

	// Handle RRCI cases
	if (instruction_type == RRCI) {
		// Get conditional
		uint8_t conditional = instruction >> 14 & 0b111;
		instruction_data.conditional = conditional;

		// Get Immediate Value
		uint32_t immediate = instruction & 0x1FFFF; // 17bit immediate
		instruction_data.immediate = immediate;
		return instruction_data;
	}

	printf("Instruction undefined. Memory value: 0x%08X\n", instruction);
	exit(1);
}

INSTRUCTION_FORMAT decode_opcode(uint32_t instruction, Instruction* instruction_type) {
	uint8_t opcode = instruction >> 27;

	switch (opcode) {
		case 0:  return RRR;   // ADD
		case 1:  return RRI;   // ADDI
		case 2:  return RRR;   // OR
		case 3:  return RRR;   // XOR
		case 4:  return RRR;   // SHLL
		case 5:  return RRR;   // SHLR
		case 6:  return RR;    // NEG
		case 7:  return RRR;   // AND
		case 8:  return RRI;   // LW
		case 9:  return RRI;   // SW
		case 10: return RRCI;  // BRC
		case 11: return RR;    // JALR
		case 12: return R;	   // PUSH
		case 13: return R;     // POP
		case 14: return RI;    // LUI
		case 15: return RR;    // SIRA

		case 17: return RRI;   // ORI
		case 18: return R;     // CSRW
		case 19: return R;     // CSRR
		default: return NULL;
	}
}