#include <stdint.h>
#include <stdio.h>
#include <stdlib.h>
#include <string.h>

#include "memory.h"

uint32_t* translate_address(uint32_t address, uint32_t* offset);
void allocate_page(uint32_t page_index);
void initialize_memory(void);
void free_memory(void);

// Public things
uint32_t readRAM(uint32_t address) {
	uint32_t offset;
	uint32_t* page = translate_address(address, &offset);
	return page[offset];
}

void writeRAM(uint32_t address, uint32_t data) {
	uint32_t offset;
	uint32_t* page = translate_address(address, &offset);
	page[offset] = data;
}

// Helper Functions

uint32_t* translate_address(uint32_t address, uint32_t* offset) {
	// Find page and offset

	uint32_t page_index = address / PAGE_SIZE;
	*offset = address % PAGE_SIZE;

	// Verify that the address is in range
	if (page_index >= NUM_PAGES) {
		printf("Address out of range: 0x%08X\n", address);
		exit(1);
	}

	// Allocate page if it doesn't exist yet
	if (mem.pages[page_index] == NULL) {
		allocate_page(page_index);
	}
	
	return mem.pages[page_index];
}

void allocate_page(uint32_t page_index) {
	// Allocate the page if empty
	if (mem.pages[page_index] == NULL) {
		mem.pages[page_index] = (uint32_t*)malloc(PAGE_SIZE * sizeof(uint32_t));
		
		// Verify that the page exists.
		if (mem.pages[page_index] == NULL) {
			printf("Memory allocation failed for page %u\n", page_index);
			exit(1);
		}

		// Set all values to 0;
		memset(mem.pages[page_index], 0, PAGE_SIZE * sizeof(uint32_t)); 
	}
}

void initialize_memory() {
	for (int i = 0; i < NUM_PAGES; i++) {
		mem.pages[i] = NULL;
	}
}

void free_memory() {
	for (int i = 0; i < NUM_PAGES; i++) {
		if (mem.pages[i] != NULL) {
			free(mem.pages[i]);
			mem.pages[i] = NULL;
		}
	}
}