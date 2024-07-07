cmake_minimum_required(VERSION 3.29)
project(OppoT2Emulator C)

set(CMAKE_C_STANDARD 99)

add_executable(OppoT2Emulator main.c
        memory.c
        memory.h
        instruction.c
        instruction.h)
