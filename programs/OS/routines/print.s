# Print assumes that r18 contains the start location of the string to print.
# t1 contains the address to TTY
# t0 contains the character to print

.print 
    # save where we jumped from
    push ra
    movi t1, 0xFFFFFFFF         # Store the location of the TTY

.printLoop
    lw t0, r18, 0                # Load the character
    brc r0, t0, EQ, done        # If null value, hang
    sw t0, t1, 0                # Store character to the TTY
    addi r18, r18, 1              # Increment string
    brc r0, r0, EQ, printLoop   # Continue printing.

.done 
    pop ra
    jalr r0, ra

