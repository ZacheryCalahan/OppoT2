#include <stdio.h>
#include <stdint.h>
#include <stdlib.h>
#include <string.h>
#include <stdbool.h>

#include "memory.h"
#include "instruction.h"

struct CPU_STATE {
	// Program Counter
	uint32_t pc;
	uint32_t isrSaveReg;

	// Registers
	uint32_t* regFile[32];

	
} CPU_STATE;

void init();
void emulate_cycle(uint32_t instruction);

int main(int argc, char* argv[]) {
	init();

	// Verify that instructions are valid.
	uint32_t instruction = 0x58440001; // jalr r1, r2, 0x1
	Instruction_Data instData = decode_instruction(instruction);

	printf("OPCODE: %d", instData.opcode);
	printf("rA: %d", instData.regA);
	printf("rB: %d", instData.regB);
	printf("rC: %d", instData.regC);
	printf("immediate: %d", instData.immediate);
	printf("conditional: %d", instData.conditional);
	return 0;
}

// Set all portions of the CPU state to zero.
void init() {
	CPU_STATE.isrSaveReg = 0;
	CPU_STATE.pc = 0;

	for (int i = 0; i < 32; i++) {
		CPU_STATE.regFile[i] = 0;
	}

	initialize_memory();
}

void emulate_cycle(uint32_t instruction) {

}