# MouseCapture
MouseCapture is a small WPF application (targeting .NET 8) that captures low-level mouse left-button single-click events on Windows and displays a timestamp and the application/window that received the click.

## Features
- Captures left mouse button single clicks using a low-level Windows hook.
- Displays timestamp and application/window details for each click.
- Start/Stop capturing through MVVM `StartCommand` and `StopCommand`.
- Bounded event history shown in the UI.

## Getting Started

## Assumptions
- Running on Windows (desktop) — low-level mouse hooks are platform-specific.
- No elevated privileges are required for the hook in normal scenarios, but some environments/antivirus may interfere.
- The application is intended only to record the mouse left button single clicks along with application clicked.

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- Windows desktop OS
- Visual Studio 2022 or later (recommended) for XAML tooling

## Usage
1. Launch the app.
2. Click Start to begin capturing. Click Stop to end capture.
3. Events appear in the list with timestamp and the application/window name.
  
   
   
