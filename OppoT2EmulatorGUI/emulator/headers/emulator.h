#ifndef EMULATOR_H
#define EMULATOR_H

#include <stdint.h>
#include <stdio.h>
struct CPU_STATE {
    // Program Counter
    uint32_t pc;
    uint32_t isrSaveReg;

    // Registers
    uint32_t regFile[32];

    // Control Registers
    uint32_t csr;
    uint32_t interrupt_source_register;

    // External Control Lines
    int irq;
};

#define DEBUG TRUE

#endif


// Initialize the Emulator
void init();

// Emulate a clock cycle
void emulate_cycle(void);

// Read a binary stream to Memory
void read_bin_to_memory(FILE *fptr);

// Write to Memory
void writeMem(uint32_t address, uint32_t data);

// Read from Memory
uint32_t readMem(uint32_t address);





