# This program prints ascii text to the TTY.

.start
    movi t0, text               # Store the location of the start of the string
    movi t1, 0xFFFFFFFF         # Store the location of the TTY

.printLoop
    lw s0, t0, 0                # Load the character
    brc r0, s0, EQ, hang        # If null value, hang
    sw s0, t1, 0                # Store character to the TTY
    addi t0, t0, 1              # Increment string
    brc r0, r0, EQ, printLoop   # Continue printing.

.hang
    brc r0, r0, EQ, hang

.text
    @ascii Hello, World!