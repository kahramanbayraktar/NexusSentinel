# Blazor UI & Real-time Dashboard Mechanics

The **NexusSentinel** dashboard is designed to provide a premium, real-time visualization of IoT telemetry. This required careful handling of Blazor's lifecycle and state management.

## ⏱️ Real-time Updates with Timers
To create a "live" feel without overwhelming the server, we implemented a dual-task timer strategy in `Telemetry.razor`.

### Implementation:
- **System.Threading.Timer:** Used a high-frequency timer (every 100ms) to update a UI-only progress bar.
- **State Synchronization:** Every time the progress bar reaches 100%, a data refresh is triggered.

```csharp
_mainTimer = new Timer(async _ => 
{
    progressValue += 2; // Smooth progress bar movement
    if (progressValue >= 100)
    {
        progressValue = 0;
        await RefreshData(); 
    }
    await InvokeAsync(StateHasChanged);
}, null, 0, 100);
```

**Why `InvokeAsync(StateHasChanged)`?**
Timers run on background threads. Blazor's UI can only be updated from the UI thread. `InvokeAsync` ensures the UI update is dispatched correctly, preventing threading exceptions.

## 🎨 Premium UI Strategy
Instead of a basic table, we focused on "Visual Excellence":
- **Dynamic Stats Cards:** Average temperature and humidity are calculated on-the-fly and displayed in glassmorphism-style cards.
- **Conditional Styling:** Temperature values change color (`temp-cold`, `temp-warm`, etc.) based on their value using C# switch expressions.
- **Micro-animations:** We added a `row-refresh-animation` class to the table body to make data updates feel smooth rather than jarring.

## 🧩 Challenges: Lifecycle & Disposal
**The Issue:** If you don't stop a timer when navigating away from a page, it keeps running in the background, causing memory leaks and potential crashes.

**The Solution:** 
1. Implementing `@implements IDisposable`.
2. Explicitly disposing the timer in the `Dispose()` method.

```csharp
public void Dispose()
{
    _mainTimer?.Dispose();
}
```

## 🌍 Lessons in Scalability
While polling (timers) works for an MVP, we discussed that the next step for a production system would be **SignalR** or **gRPC Streaming**, where the server "pushes" updates to the client only when new data arrives.
