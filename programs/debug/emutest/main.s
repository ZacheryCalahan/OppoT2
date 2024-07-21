.start
    movi sp, 0xFFFFFFFF     # Set Stack pointer
    movi t0, 0x80000000     # Disable ints
    csrw t0
    movi t0, put_string     # Store address of the put_string function
    movi r18, hello         # Store the address of the ASCII string to print
    jalr ra, t0             # Perform put_string
    add r0, r0, r0          # Filler
.halt 
    brc r0, r0, eq, halt    # Hang

# Prints the char in r18
.put_char
    movi t1, 0x80000200     # Location of TTY Output
    sw r18, t1, 0           # Output the char to the TTY
    jalr r0, ra             # Return

# Prints a string from the pointer in r18
.put_string
    push ra
    push s0
    .put_string_loop
        movi s0, put_char                   # Save location of put_char

        push r18                            # Save the location of the string pointer
        lw r18, r18, 0                      # Load character
        brc r0, r18, eq, put_string_done    # Check if char is 0, meaning string terminated
        jalr ra, s0
        pop r18                             # Retrieve pointer to the string
        addi r18, r18, 1                    # Increment pointer to the string
        brc r0, r0, eq, put_string_loop     # Continue loop

    .put_string_done
        pop r0                              # Pop extra var out from r18
        pop s0
        pop ra
        jalr r0, ra             

@org 0x0000BEEF
.hello
    @ascii "Hello, World!"