# Contributing to Honua Mobile SDK

We love your input! We want to make contributing to the Honua Mobile SDK as easy and transparent as possible, whether it's:

- Reporting a bug
- Discussing the current state of the code
- Submitting a fix
- Proposing new features
- Becoming a maintainer

## Development Process

We use GitHub to host code, to track issues and feature requests, as well as accept pull requests.

## Pull Requests

Pull requests are the best way to propose changes to the codebase. We actively welcome your pull requests:

1. Fork the repo and create your branch from `main`.
2. If you've added code that should be tested, add tests.
3. If you've changed APIs, update the documentation.
4. Ensure the test suite passes.
5. Make sure your code lints.
6. Issue that pull request!

## Development Setup

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) or later
- [Visual Studio 2022 17.8+](https://visualstudio.microsoft.com/) or [VS Code](https://code.visualstudio.com/)
- Platform-specific SDKs (see [Platform Setup](docs/platform-setup.md))

### Local Development

1. **Clone the repository:**
   ```bash
   git clone https://github.com/mikemcdougall/honua-mobile-sdk.git
   cd honua-mobile-sdk
   ```

2. **Install MAUI workload:**
   ```bash
   dotnet workload install maui
   ```

3. **Restore dependencies:**
   ```bash
   dotnet restore
   ```

4. **Build the solution:**
   ```bash
   dotnet build
   ```

5. **Run tests:**
   ```bash
   dotnet test
   ```

### Platform-Specific Development

#### Android
```bash
# Build for Android
dotnet build -f net8.0-android

# Run example app
dotnet run --project examples/FieldDataCollection -f net8.0-android
```

#### iOS (macOS only)
```bash
# Build for iOS Simulator
dotnet build -f net8.0-ios

# Run in iOS Simulator
dotnet run --project examples/FieldDataCollection -f net8.0-ios
```

#### Windows
```bash
# Build for Windows
dotnet build -f net8.0-windows10.0.19041.0

# Run Windows app
dotnet run --project examples/FieldDataCollection -f net8.0-windows10.0.19041.0
```

## Code Style

We use standard .NET formatting conventions. The project includes an `.editorconfig` file that defines our style rules.

### Formatting

Before submitting, ensure your code is properly formatted:

```bash
# Format code
dotnet format

# Check formatting
dotnet format --verify-no-changes
```

### Code Analysis

We use .NET analyzers for code quality. Fix any analyzer warnings before submitting:

```bash
# Run analyzers
dotnet build --verbosity normal
```

## Testing

### Unit Tests

All new features should include unit tests:

```bash
# Run all tests
dotnet test

# Run tests with coverage
dotnet test --collect:"XPlat Code Coverage"
```

### Platform Testing

Test on multiple platforms when possible:

```bash
# Test on Android emulator
dotnet test --logger trx -f net8.0-android

# Test on iOS simulator (macOS only)
dotnet test --logger trx -f net8.0-ios
```

## Documentation

### API Documentation

- Use XML documentation comments for all public APIs
- Include examples in documentation where helpful
- Update README.md for significant changes

### Code Comments

- Write self-documenting code when possible
- Add comments for complex business logic
- Explain "why" not "what" in comments

## Issue Reporting

We use GitHub issues to track public bugs. Report a bug by [opening a new issue](https://github.com/mikemcdougall/honua-mobile-sdk/issues/new).

### Bug Reports

**Great Bug Reports** tend to have:

- A quick summary and/or background
- Steps to reproduce
  - Be specific!
  - Give sample code if you can
- What you expected would happen
- What actually happens
- Notes (possibly including why you think this might be happening, or stuff you tried that didn't work)

### Feature Requests

We welcome feature requests! Please provide:

- Clear description of the feature
- Use case or motivation
- Proposed API (if applicable)
- Implementation considerations

## Platform-Specific Guidelines

### Android

- Follow [Android development best practices](https://developer.android.com/guide)
- Test on multiple Android versions when possible
- Consider performance on lower-end devices

### iOS

- Follow [iOS Human Interface Guidelines](https://developer.apple.com/design/human-interface-guidelines/)
- Test on both iPhone and iPad when applicable
- Consider iOS-specific features and limitations

### Windows

- Follow [Windows App Development Guidelines](https://docs.microsoft.com/en-us/windows/apps/)
- Consider desktop and tablet form factors
- Test with different input methods (touch, mouse, keyboard)

## Mobile-Specific Considerations

### Performance

- Consider battery usage impact
- Test with limited network connectivity
- Profile memory usage
- Test on actual devices when possible

### Offline Functionality

- Ensure offline features work without network
- Test sync conflict resolution
- Consider storage limitations

### User Experience

- Follow platform UI guidelines
- Consider accessibility requirements
- Test with different screen sizes
- Support both portrait and landscape orientations

## Commit Messages

Use clear and descriptive commit messages:

```
feat: add camera capture with geotagging
fix: resolve map rendering issue on Android
docs: update getting started guide
test: add unit tests for location service
refactor: simplify offline sync logic
```

### Commit Types

- `feat`: New feature
- `fix`: Bug fix
- `docs`: Documentation changes
- `test`: Adding or updating tests
- `refactor`: Code refactoring
- `style`: Code style changes
- `perf`: Performance improvements
- `chore`: Maintenance tasks

## Code Review Process

1. **Automated Checks**: All PRs must pass automated tests and code quality checks
2. **Peer Review**: At least one maintainer must review and approve changes
3. **Platform Testing**: Test on relevant platforms
4. **Documentation**: Ensure documentation is updated for public API changes

## License

By contributing, you agree that your contributions will be licensed under the Apache License 2.0.

## Getting Help

- 📚 [Documentation](docs/getting-started.md)
- 💬 [Discussions](https://github.com/mikemcdougall/honua-mobile-sdk/discussions)
- 🐛 [Issues](https://github.com/mikemcdougall/honua-mobile-sdk/issues)

## Recognition

Contributors will be recognized in:
- CONTRIBUTORS.md file
- Release notes for significant contributions
- GitHub contributor stats

Thank you for contributing to the Honua Mobile SDK! 🙏