cmake_minimum_required(VERSION 3.29)
project(OppoT2Disassembler C)

set(CMAKE_C_STANDARD 99)

add_executable(OppoT2Disassembler main.c
        memory.c
        headers/memory.h
        disassembler.c
        headers/disassembler.h
        instruction.c
        headers/instruction.h)

target_include_directories(OppoT2Disassembler PUBLIC headers)