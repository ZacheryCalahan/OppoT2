.entrypoint
    movi sp, 0xFFFFFFFF     # Initialize the stack pointer at location 
    movi r29, kernelMain    # Load code location into a register
    jalr r0, r29            # Long jump to kernel

@org 0x2000 # Start of ISR
    @include C:\Users\zache\Documents\Coding\CPU\OppoT2\programs\OS\kernel\drivers\isr.s
@org 0x3000 # Start address of open memory
    @include C:\Users\zache\Documents\Coding\CPU\OppoT2\programs\OS\kernel\kernel.s


