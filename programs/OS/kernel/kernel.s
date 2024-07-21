# Save drivers
@include C:\Users\zache\Documents\Coding\CPU\OppoT2\programs\OS\kernel\drivers\keyboard.s
@include C:\Users\zache\Documents\Coding\CPU\OppoT2\programs\OS\kernel\drivers\screen.s

.kernelMain
    # Perform setup
    movi t0, keyboard_init      # Initialize the keyboard driver
    jalr ra, t0

    movi r18, hello             # Store the location of the start of the string
    movi s0, put_string         # Load location of print
    jalr ra, s0                 # Perform print call

    .inputLoop
        movi t0, keyboard_get_char
        jalr ra, t0                     # Get the char at the pointer
        brc r22, r0, eq, inputLoop      # Jump to imput loop if no char
        movi t0, put_char               # Save location of put_char
        add r18, r22, r0                # Move get_char output to function arg 0
        jalr ra, t0                     # Put char
        brc r0, r0, eq, inputLoop       # Loop

.hang
    brc r0, r0, eq, hang

.hello
    @ascii "Welcome to OppoOS (OppoT2)!\n"

.commandPrompt
    @ascii "Enter a command.\n"