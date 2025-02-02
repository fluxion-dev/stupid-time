using System;
using System.Reactive;
using Avalonia.Threading;
using ReactiveUI;

namespace SimpleTime;

public class MainWindowViewModel : ReactiveObject
{
    private const int RefreshMilliseconds = 10;
    
    private readonly DispatcherTimer _refreshTimer;
    
    private double _secondsSweepAngle;
    private double _minutesSweepAngle;
    private double _hoursSweepAngle;

    private TimeSpan _timeSpan = TimeSpan.Zero;

    public MainWindowViewModel()
    {
        _refreshTimer = new DispatcherTimer(
            TimeSpan.FromMilliseconds(RefreshMilliseconds),
            DispatcherPriority.Render,
            OnDispatcherTimer);

        _refreshTimer.Stop();

        StartStopCommand = ReactiveCommand.Create(StartStopCommandExecute);
        ResetCommand = ReactiveCommand.Create(ResetCommandExecute);
    }
    
    public double SecondsSweepAngle
    {
        get => _secondsSweepAngle;
        set => this.RaiseAndSetIfChanged(ref _secondsSweepAngle, value);
    }

    public double MinutesSweepAngle
    {
        get => _minutesSweepAngle;
        set => this.RaiseAndSetIfChanged(ref _minutesSweepAngle, value);
    }

    public double HoursSweepAngle
    {
        get => _hoursSweepAngle;
        set => this.RaiseAndSetIfChanged(ref _hoursSweepAngle, value);
    }

    public string RenderedTime => $"{_timeSpan.Hours}h {_timeSpan.Minutes}m {_timeSpan.Seconds}s";
    
    public string ButtonText => _refreshTimer.IsEnabled ? "Pause" : "Start";
    
    public ReactiveCommand<Unit, Unit> StartStopCommand { get; }
    
    public ReactiveCommand<Unit, Unit> ResetCommand { get; }

    private static double CalculatedAngleFor(double value) => Math.Floor(value / 60 * 360);
    
    private void OnDispatcherTimer(object? sender, EventArgs e) => Refresh();

    private void Refresh()
    {
        RecalculateTimespan();
        RenderArcs();
        RenderControls();
    }

    private void RenderControls()
    {
        this.RaisePropertyChanged(nameof(ButtonText));
        this.RaisePropertyChanged(nameof(RenderedTime));
    }

    private void RenderArcs()
    {
        SecondsSweepAngle = CalculatedAngleFor(_timeSpan.Seconds);
        MinutesSweepAngle = CalculatedAngleFor(_timeSpan.Minutes);
        HoursSweepAngle = CalculatedAngleFor(_timeSpan.TotalHours);
    }

    private void RecalculateTimespan()
    {
        _timeSpan = _timeSpan.Add(TimeSpan.FromMilliseconds(RefreshMilliseconds));
    }

    private void StartStopCommandExecute()
    {
        if (!_refreshTimer.IsEnabled)
            _refreshTimer.Start();
        else
            _refreshTimer.Stop();
        
        Refresh();
    }
    
    private void ResetCommandExecute()
    {
        _refreshTimer.Stop();
        _timeSpan = TimeSpan.Zero;
        Refresh();
    }
}