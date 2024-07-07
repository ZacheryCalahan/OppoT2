.debugStart
    movi t0, 0xFFFFFFFF # Location of TTY
    movi sp, 0xFFFF0000 # Set Stack Location
    movi r18, hello     # Store the location of the start of the string
    movi s0, print      # Store location of print
    jalr ra, s0         # Perform print call

.halt
    brc r0, r0, PASS, halt

# ASCII
.hello
    @ascii "Hello world!\n"

# Print
.print 
    #push ra                     # save where we jumped from
    movi t1, 0xFFFFFFFF         # Store the location of the TTY

.printLoop
    lw t0, r18, 0               # Load the character
    brc r0, t0, EQ, printDone   # If null value, finish call 32
    sw t0, t1, 0                # Store character to the TTY
    addi r18, r18, 1            # Increment string location
    brc r0, r0, EQ, printLoop   # Continue printing.

.printDone
    #pop ra
    jalr r0, ra




