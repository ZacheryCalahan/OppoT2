#pragma once

#include <stdint.h>

#define PAGE_SIZE (1 << 10) // 1KB page size
#define NUM_PAGES (1 << 22) // 4 GB of address space

// Structure holding the arrays of memory pages.
typedef struct Memory {
    uint32_t* pages[NUM_PAGES];
} Memory;

// Read a 32-bit word from RAM.
uint32_t readRAM(uint32_t address);

// Write a 32-bit word to RAM.
void writeRAM(uint32_t address, uint32_t data);

// Initialize the memory module.
void initialize_memory(void);

// Clean and free the memory of the memory module.
void free_memory(void);

// Translate a position in a page to an address
uint32_t page_to_address(uint32_t page, uint32_t offset);

// Translate an address to a position in a page.
uint32_t address_to_page(uint32_t address, uint32_t* offset);


