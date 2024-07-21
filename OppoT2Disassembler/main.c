#include <assert.h>
#include <stdint.h>
#include <stdio.h>
#include <stdlib.h>

#include "disassembler.h"
#include "memory.h"

int main(int argc, char *argv[]) {
    // Get the file
    FILE *fptr = fopen(argv[1], "r");
    if (fptr == NULL) {
        printf("Error opening file\n");
        exit(1);
    }

    // Get length of file
    fseek(fptr, 0, SEEK_END);
    const long len = ftell(fptr);
    rewind(fptr);
    uint32_t address = 0;

    for (int i = 0; i < len / 4; i++) {
        uint32_t data = fgetc(fptr) << 24;
        data |= fgetc(fptr) << 16;
        data |= fgetc(fptr) << 8;
        data |= fgetc(fptr);
        writeRAM(address, data);
        address++;
    }

    fclose(fptr);

    // Disassemble
    generate_file(argv[2]);
}