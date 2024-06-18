# comment
.entrypoint
    # loc 0
    movi sp, 0xF00 # Initialize the stack pointer at location 
    movi r29, kernelMain # Load code location into a register

    jalr r0, r29    # Long jump to .code

@org 0x1fff # Addr of the Interrupt Service Routine
.intServiceRoutine
    movi r27, 0x80000000 # Set CSR to disable ints
    csrw r27

    sira ra # Save return location

    addi s3, r0, 0b11111111111111110
    addi s4, r0, 0b11111111111111111
    lw s5, s3, 0                    
    sw s5, s4, 0                    

    # Enable ints again
    csrw r0
    jalr r0, ra

@org 0x3000 # Start address of open memory
    @include C:\Users\zache\Documents\Coding\CPU\OppoT2\programs\OS\kernel\kernel.s

.routines
    @include C:\\Users\\zache\\Documents\\Coding\\CPU\\OppoT2\\programs\\OS\\routines\\print.s
    @include C:\Users\zache\Documents\Coding\CPU\OppoT2\programs\OS\routines\saveReg.s


