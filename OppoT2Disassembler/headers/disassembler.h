#pragma once
#include <stdint.h>

char* disassembleInstruction(uint32_t instruction);

void generate_file(const char* file_path);
