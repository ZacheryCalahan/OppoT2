#ifndef EMULATOR_H
#define EMULATOR_H

#include <stdint.h>
struct CPU_STATE {
    // Program Counter
    uint32_t pc;
    uint32_t isrSaveReg;

    // Registers
    uint32_t regFile[32];
    uint32_t csr;

};
#endif


// Initialize the Emulator
void init();

// Emulate a clock cycle
void emulate_cycle(void);

// Read a binary stream to Memory
void read_bin_to_memory(FILE *fptr);

// Emulate an Interrupt
void handle_interrupts(uint32_t , uint32_t data);

// Write to Memory
void writeMem(uint32_t address, uint32_t data);

// Read from Memory
uint32_t readMem(uint32_t address);





