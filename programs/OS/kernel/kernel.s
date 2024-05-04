.kernelMain
    movi r18, hello             # Store the location of the start of the string
    movi s0, print              # Load location of print
    jalr ra, s0                 # Perform print call
    
    movi r18, commandPrompt     # Store string loc
    movi s0, print              # Load loc of print routine
    jalr ra, s0                 # Perform print call

.hang
    brc r0, r0, eq, hang # 8191

.hello
    @ascii "Welcome to OppoOS (OppoT2)!\n"

.commandPrompt
    @ascii "Enter a command.\n"