#include <stdio.h>
#include <stdlib.h>
#include "emulator.h"
#include <time.h>
#include <windows.h>
#include <gtk/gtk.h>
#include "main.h"
#include <interface.h>

extern struct CPU_STATE CPU_STATE;

HANDLE StartEmulator();

GtkWidget *fixed1;
GtkWidget *tty_output_box;
GtkTextBuffer *output_buffer;
boolean CLK_STEP = FALSE;
boolean CLK_APROP = FALSE;

static void quit_cb(GtkWindow *window) {
    gtk_window_close(window);
}

static gboolean key_press_event_cb(GdkEvent *event, guint keyval, guint keycode, GdkModifierType state, gpointer user_data) {
    const char *key_name = gdk_keyval_name(keyval);
    const uint32_t c = gdk_keyval_to_unicode(keyval);

    // Ensure character is an ASCII character.
    if (c < 256) {
        HandleKeyboard(c);
    }

    return true;
}

static void clock_toggle_cb(GtkToggleButton *button, gpointer data) {
    CLK_APROP = gtk_toggle_button_get_active(button);
}

static void clock_step_cb(GtkButton *button, gpointer data) {
    CLK_STEP = TRUE;
}

void update_register_values(GtkWidget **value_labels) {
    char buffer[20];

    // Get general purpose registers
    for (int i = 0; i < 32; i++) {
        snprintf(buffer, sizeof(buffer), "%08X", CPU_STATE.regFile[i]);
        gtk_label_set_text(GTK_LABEL(value_labels[i]), buffer);
    }

    // PC
    snprintf(buffer, sizeof(buffer), "%08X", CPU_STATE.pc);
    gtk_label_set_text(GTK_LABEL(value_labels[32]), buffer);

    snprintf(buffer, sizeof(buffer), "%08X", CPU_STATE.csr);
    gtk_label_set_text(GTK_LABEL(value_labels[33]), buffer);
}

void write_to_tty(const char c) {
    gtk_text_buffer_insert_at_cursor(output_buffer, &c, 1);
}

GtkWidget *value_labels[34];
static void activate(GtkApplication *app, gpointer user_data) {
    // Init builder
    GtkBuilder *builder = gtk_builder_new_from_file("../window.ui");

    // Get window
    GtkWidget *window = GTK_WIDGET(gtk_builder_get_object(builder, "window"));

    // Add window to the app w/ signals
    gtk_window_set_application(GTK_WINDOW(window), app);
    g_signal_connect(window, "close-request", G_CALLBACK(quit_cb), NULL);

    // Get components
    GtkWidget *fixed1 = GTK_WIDGET(gtk_builder_get_object(builder, "fixed1"));
    GtkWidget *tty_output_box = GTK_WIDGET(gtk_builder_get_object(builder, "tty_output_box"));

    // Setup File Chooser


    // Setup handler for keyboard input
    GtkEventControllerKey *keyboard_handler = GTK_EVENT_CONTROLLER_KEY(gtk_event_controller_key_new());
    g_signal_connect(keyboard_handler, "key-pressed", G_CALLBACK(key_press_event_cb), NULL);
    gtk_widget_add_controller(window, GTK_EVENT_CONTROLLER(keyboard_handler));

    // Setup output box for writing
    output_buffer = gtk_text_buffer_new(NULL);
    gtk_text_view_set_buffer(GTK_TEXT_VIEW(tty_output_box), output_buffer);
    gtk_text_view_set_editable(GTK_TEXT_VIEW(tty_output_box), FALSE);

    // Setup debug view of each register
    GtkWidget *grid = gtk_grid_new();

    gtk_widget_set_halign(grid, GTK_ALIGN_START);
    gtk_fixed_put(GTK_FIXED(fixed1), grid, 800, 100);
    GtkWidget *name_labels[32];

    for (int i = 0; i < 32; i++) {
        char name[20];
        snprintf(name, sizeof(name), "Register %d:  ", i);

        name_labels[i] = gtk_label_new(name);
        value_labels[i] = gtk_label_new("");

        gtk_grid_attach(GTK_GRID(grid), name_labels[i], 0, i, 1, 1);
        gtk_grid_attach(GTK_GRID(grid), value_labels[i], 1, i, 1, 1);
    }
    value_labels[32] = gtk_label_new("");
    value_labels[33] = gtk_label_new("");
    GtkWidget *pcLabel = gtk_label_new("PC:  ");
    GtkWidget *csrLabel = gtk_label_new("CSR:  ");
    gtk_grid_attach(GTK_GRID(grid), pcLabel, 0, 32, 1, 1);
    gtk_grid_attach(GTK_GRID(grid), value_labels[32], 1, 32, 1, 1);
    gtk_grid_attach(GTK_GRID(grid), csrLabel, 0, 33, 1, 1);
    gtk_grid_attach(GTK_GRID(grid), value_labels[33], 1, 33, 1, 1);

    // Clock toggle
    GtkWidget *clk_toggle = GTK_WIDGET(gtk_builder_get_object(builder, "clk_toggle"));
    g_signal_connect(clk_toggle, "toggled", G_CALLBACK(clock_toggle_cb), NULL);

    // Clock step button
    GtkWidget *clk_step_button = GTK_WIDGET(gtk_builder_get_object(builder, "clk_step"));
    g_signal_connect(clk_step_button, "clicked", G_CALLBACK(clock_step_cb), NULL);

    // Setup window
    gtk_window_set_title(GTK_WINDOW(window), "Emulator");
    gtk_window_set_default_size(GTK_WINDOW(window), 1000, 900);
    gtk_window_present(GTK_WINDOW(window));

    g_object_unref(builder);

}

int main(int argc, char *argv[]) {
    // Start the emulator and load memory. (Make the init a file chooser later.)
    // ALSO, you already know my name from my github. Don't flame me for the hardcoded file path, you're just a prude.
    FILE *fptr = fopen("C:\\Users\\zache\\Documents\\Coding\\CPU\\OppoT2\\out\\out.o", "r");
    init();
    read_bin_to_memory(fptr);

    HANDLE emulatorThread = StartEmulator();

    // Create the GUI

#ifdef GTK_SRCDIR
    g_chdir (GTK_SRCDIR)
#endif

    GtkApplication *app = gtk_application_new("org.gtk.example", G_APPLICATION_DEFAULT_FLAGS);
    g_signal_connect(app, "activate", G_CALLBACK(activate), NULL);

    int status = g_application_run (G_APPLICATION (app), argc, argv);
    g_object_unref(app);

    return status;

}

// Emulator Stuff

#define CLK_SPEED 0
DWORD WINAPI EmulatorThread(void* data) {
    Sleep(1000); // Sleep for 1 second to wait for the GUI to init.
    // This is a thread with the emulator.
    clock_t start = clock();

    while (1) {
        // Run a cycle @ clock speed

        const int elapsed = clock() - start;
        if (elapsed >= CLK_SPEED && CLK_APROP) {
            // Auto Propagate mode
            emulate_cycle();
            start = clock();
        } else if (CLK_STEP) {
            // Clock Step Mode
            emulate_cycle();
            CLK_STEP = FALSE;
            update_register_values(value_labels);
        }
    }
}

// Start the thread of the Emulator, and return its handler.
HANDLE StartEmulator() {
    // Start the emulator.
    return CreateThread(NULL, 0, EmulatorThread, NULL, 0, NULL);
}




