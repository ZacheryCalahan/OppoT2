
# Initialize the keyboard and set up the driver
.keyboard_init
    # Initialize the keyboard buffer
    movi t0, 0x80000100     # Pointer to the start of the buffer
    movi t1, 0x800001FF     # Memory location of the buffer index pointer (LIFO data structure)
    sw t0, t1, 0            # Initialize pointer to the start of the buffer
    jalr r0, ra             # Return

# Assumes r18 contains a char to save.
.keyboard_put_char
    movi t0, 0x800001FF     # Memory location of the buffer index pointer
    lw t1, t0, 0            # Load the location of the pointer

    sw r18, t1, 0           # Save the char to the buffer
    addi t1, t1, 1              # Increment the buffer pointer
    sw t1, t0, 0            # Save new buffer pointer to memory
    jalr r0, ra             # Return

# Returns a char in r22 from the buffer
.keyboard_get_char
    movi t0, 0x800001FF     # Memory location of the buffer index pointer
    lw t1, t0, 0            # Index pointer

    movi t2, 0x80000100     # Check to see if buffer is empty
    brc t1, t2, eq, keyboard_get_char_done

    lw r22, t1, 0           # Load char from buffer
    movi t3, 0xFFFFFFFF     # Save -1
    add t1, t0, t3          # Decrement the buffer pointer
    sw t0, t1, 0            # Save new buffer pointer to memory
    jalr r0, ra             # Return

    .keyboard_get_char_done
        and r22, r0, r0     # Set return to 0
        jalr r0, ra         # Return
