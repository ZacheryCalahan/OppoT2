# comment
.entrypoint
    # loc 0
    movi sp, 0xff # Initialize the stack pointer at location 
    movi r29, kernelMain # Load code location into a register

    jalr r0, r29    # Long jump to .code

@org 0x1fff # Addr of the Interrupt Service Routine
.intServiceRoutine
    push t0
    push t1
    push t2

    # Check for device ID
    movi t0, 0x0f000000  # load value of keyboard id
    movi t1, 0xff000000  # get device ID only
    and t1, t1, t0
    brc t1, t0, eq, onKeypress
    brc r0, r0, eq, reset

    .onKeypress
        movi t0, 0xFFFFFFFE # load address of keyboard
        movi t1, 0xFFFFFFFF # load address of TTY
        lw t0, t2, 0 # load incoming key
        # save to buffer
        # inc buffer location

                           
        brc r0, r0, eq, isrFinished

    .reset
        jalr r0, r0
        

    .isrFinished
        pop t2
        pop t1
        pop t0
        isrr

@org 0x3000 # Start address of open memory
    @include C:\Users\zache\Documents\Coding\CPU\OppoT2\programs\OS\kernel\kernel.s

.routines
    @include C:\\Users\\zache\\Documents\\Coding\\CPU\\OppoT2\\programs\\OS\\routines\\print.s
    @include C:\Users\zache\Documents\Coding\CPU\OppoT2\programs\OS\routines\saveReg.s


