.intServiceRoutine
    # Set CSR to disable ints and save return location
    movi r27, 0x80000000 
    csrw r27
    sira ra

    # Push all used registers to the stack
    push t0
    push t1
    push t2
    push t3

    # Get the controller
    addi t0, t0, 1
    addi t1, t1, 1

    brc isr, t0, eq, peripheralISR
    shll t0, t0, t1
    brc r27, t0, eq, storageISR

    # Enable ints again (port check failed, don't handle data.)
    csrw r0
    jalr r0, ra

.peripheralISR
    # Get the port to handle (handle the lowest first)
    movi t0, 0x80000000 # Mem location of peripheral controller info
    lw t1, t0, 0        # Save port data to t1
    
    addi t2, t2, 1          # value to check against
    addi t3, t3, 0          # counter

    .peripheralPortSearch
        and t0, t2, t1  # isolate bit
        brc t0, t2, eq, port0ISR
        shlr t1, t1, t2
        and t0, t2, t1
        brc t0, t2, eq, port1ISR
        # etc...
        # Finish up, no port found?
        brc r0, r0, eq, isrDone


    # This is bound to the keyboard.
    .port0ISR
        push r18
        movi t0, 0x80000001             # get data from port 1
        lw r18, t0, 0                   # t1 contains the data from the keyboard
        movi t1, keyboard_put_char      # Save the location of the put char routine
        jalr ra, t1                     # Jump
        pop r18
        brc r0, r0, eq, isrDone         # Finish up call

    .port1ISR
        brc r0, r0, eq, isrDone         # Nothing to do here.


.storageISR
    # Nothing to do here.
    brc r0, r0, eq, isrDone

.isrDone
    csrw r0         # Enable interrupts again
    pop t0          # Return all used values back to their prior state
    pop t1
    pop t2
    pop t3
    pop t4
    jalr r0, ra     # Jump back to code