//
// Created by zache on 7/7/2024.
//

#pragma once

#include <stdint.h>

void init(const char* filePath);
void emulate_cycle(void);
void print_state(void);
void read_bin_to_memory(const char* file);
void handle_interrupts(uint32_t istat);
void writeMem(uint32_t address, uint32_t data);
uint32_t readMem(uint32_t address);

#define CLK_SPEED 0

