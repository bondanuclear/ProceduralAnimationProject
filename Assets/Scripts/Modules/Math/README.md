# Equation Solver System

This module provides a flexible system for implementing second-order dynamics in procedural animation. It allows switching between different mathematical methods for solving second-order differential equations.

## Available Solver Types

- **EulerStable**: Simple and efficient solver suitable for most use cases
- **EulerStableCorrectPhysics**: Improved Euler method with better physics representation
- **SemiImplicitEuler**: Semi-implicit Euler method with good balance of accuracy and performance (default)
- **VerletIntegration**: Verlet integration with excellent stability and energy conservation

## How to Change Equation Solver Type

### Before Runtime (In Editor)

Characters created with the Character Spawner have direct configuration fields for equation solver settings:

1. Select a character in the hierarchy
2. In the Inspector, find the SpiderController or MovementStateMachine component
3. Under "Equation Solver Configuration", select your desired solver type and parameters
4. The new settings will be applied when the game starts

### Using the Editor Tool

1. Select "Tools > Character Equation Solver Editor" from the Unity menu
2. Select your character from the dropdown
3. Choose the desired solver type and adjust parameters
4. Click "Apply Equation Solver" to update the character immediately

### During Spawning

When using the Character Spawner tool (Tools > Character Spawner):

1. Select your desired character type
2. Under "Equation Solver Settings", configure the solver type and parameters
3. The character will be spawned with these settings

## Parameters

- **Frequency**: Controls how quickly the system oscillates (higher = faster oscillation)
- **Damping**: Controls how quickly oscillations die out (higher = faster stabilization)
- **Response**: Controls responsiveness to input changes (higher = more responsive)

## Presets

The editor includes several presets:

- **Smooth Motion**: Lower frequency, higher damping for smooth, stable movement
- **Responsive**: Higher frequency, medium damping for quick response to input
- **Bouncy**: Medium frequency, low damping for springy, bouncy movement

## Technical Details

The equation solvers implement the `IEquationSolver` interface, which provides the `UpdateValues` method:

```csharp
Vector3 UpdateValues(Vector3 x, Vector3? xd = null, float T = Mathf.Infinity);
```

- `x`: Target position
- `xd`: Current velocity (optional)
- `T`: Time step (defaults to Time.fixedDeltaTime)

The system initializes the appropriate solver in the Awake method based on the configuration you select in the editor.

The system uses reflection to find and update equation solver instances in various components that use procedural animation, including:

- MovementContext
- MovementStateMachine
- SpiderController
- Various procedural animation rig components 