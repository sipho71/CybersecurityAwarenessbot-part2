# CybersecurityAwarenessbot-part2

A WPF chatbot (C# / .NET 8) that teaches users how to stay safe online.

## Features
- WPF GUI with ASCII art logo and a voice greeting (Audio/greeting.wav)
- Asks for your name and remembers it
- Keyword recognition: passwords, scams, privacy, phishing, malware, safe browsing
- Random tips per topic (Dictionary of Lists)
- Memory: remembers your favourite topic and personalises later tips
- Sentiment detection: worried, frustrated/confused, curious
- Follow-ups: "tell me more", "give me another tip"
- Default response for unknown input
- Delegate (`ResponseProcessor`) used to process messages

## How to run
1. Install Visual Studio with the .NET desktop development workload
2. Open `CyberSecurityAwarenessBot.csproj`
3. Press F5 to run

## Example questions
- Tell me about password safety
- Give me a phishing tip
- I'm interested in privacy
- I'm worried about online scams
- Tell me more
- What is my name?

## Structure
- `MainWindow.xaml / .xaml.cs`: GUI and message handling
- `Bot/chatbot.cs`: chatbot logic, memory, sentiment, responses
- `Audio/greeting.wav`: voice greeting

  ## Changes from Part 1
- The ASCII art logo was redesigned for the GUI version (padlock and banner design). Part 1 used a simpler logo.
