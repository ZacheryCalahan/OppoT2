
# Prints the char in r18
.put_char
    movi t1, 0x80000200     # Location of TTY Output
    sw r18, t1, 0           # Output the char to the TTY
    jalr r0, ra             # Return

# Prints a string from the pointer in r18
.put_string
    push ra
    push s0
    movi s0, put_char               # Save location of put_char
    lw t0, r18, 0                   # Load character
    brc r0, t0, eq, put_string_done # Check if char is 0, meaning string terminated
    jalr ra, s0
    addi r18, r18, 1
    brc r0, r0, eq, put_string      # Continue loop

    .put_string_done
        pop s0
        pop ra
        jalr r0, ra