# WARP.md

This file provides guidance to WARP (warp.dev) when working with code in this repository.

## Project Overview

This is a Visual Studio Code extension called "Copy Path with Code" that enables developers to copy file paths along with their content in formatted output. The extension provides advanced clipboard management, folder organization, and file detection capabilities.

**Key Capabilities:**
- Copy file paths with content in markdown-formatted output
- Organize related files into custom folders
- Detect and manage clipboard content automatically
- Temporary clipboard storage and restoration
- Multi-workspace support with visual indicators

## Development Commands

### Build and Compilation
```bash
# Install dependencies
npm install

# Development build with webpack
npm run compile

# Watch mode for development
npm run watch

# Production build (for publishing)
npm run package
```

### Testing
```bash
# Compile tests
npm run compile-tests

# Watch tests
npm run watch-tests

# Run full test suite (includes linting and compilation)
npm run pretest

# Run tests only
npm run test

# Run specific test file
npm test -- --grep "specific test pattern"
```

### Code Quality
```bash
# Run ESLint on src directory
npm run lint
```

### VS Code Extension Development
```bash
# Prepare for publishing to VS Code Marketplace
npm run vscode:prepublish

# Launch extension development host
code . && F5

# Package the extension (.vsix file)
npm run package
```

## Architecture Overview

### Core Structure
The extension follows a modular architecture with clear separation of concerns:

**`src/extension.ts`** - Main entry point that:
- Initializes all providers and services
- Registers commands and tree views
- Sets up clipboard monitoring and file watching
- Manages extension lifecycle

**State Management** (`src/models/models.ts`):
- Centralized application state in `state` object
- TypeScript interfaces for type safety
- Handles copied files, folders, clipboard detection, and temporary storage

**Command Architecture** (`src/commands/`):
- Modular command organization by functionality
- `index.ts` orchestrates all command registration
- Separate modules for core, folder, directory, context menu, and temp clipboard commands

**Data Providers** (`src/providers/`):
- `FolderTreeDataProvider` - Manages the Code Folders tree view with workspace/global modes
- `ClipboardTreeDataProvider` - Handles detected clipboard files display

**Utilities** (`src/utils/`):
- `clipboardDetector.ts` - Monitors clipboard changes and detects file content
- `fileWatcher.ts` - Tracks file system changes and cleanup
- `logger.ts` - Centralized logging system
- `folderUtils.ts` - Folder persistence and management
- `workspaceUtils.ts` - Multi-workspace handling
- `uiUtils.ts` - UI interaction helpers

### Key Extension Points

**Tree Views:**
- `folderManager` - Main Code Folders view in activity bar
- `clipboard-detection` - Clipboard Files panel in Explorer sidebar

**Commands Pattern:**
Each command module registers multiple related commands following the naming convention:
`copy-path-with-code.{commandName}`

**Webview Integration:**
The extension uses webviews for advanced file management interfaces with theme-aware styling.

### Multi-Workspace Support
The extension intelligently handles multiple VS Code workspaces:
- Folders are tagged with their originating workspace
- Visual indicators show workspace relationships
- Context switching maintains folder organization

### Clipboard System Architecture
**Three-Tier Clipboard Management:**
1. **Active Clipboard** (`state.copiedFiles`) - Current copied content
2. **Detection Queue** (`state.clipboardFiles`) - Auto-detected clipboard files
3. **Temporary Storage** (`state.tempClipboard`) - Saved clipboard state

## Configuration

### TypeScript Configuration
- Target: ES2022 with Node16 modules
- Strict type checking enabled
- Source maps for debugging

### Webpack Build
- Entry point: `./src/extension.ts`
- Output: `./dist/extension.js`
- External: vscode module (provided by VS Code runtime)
- TypeScript compilation via ts-loader

### ESLint Rules
- TypeScript ESLint integration
- Naming conventions for imports
- Standard code quality rules (curly braces, equality checks, semicolons)

## Testing Strategy

### Test Structure
- Test files located in `src/test/`
- Uses VS Code's testing framework with Mocha
- Integration with `@vscode/test-cli` and `@vscode/test-electron`

### Running Tests in VS Code
The extension includes VS Code launch configurations for:
- Running the extension in development mode
- Debugging extension tests
- Attaching to running extension processes

## Extension Packaging

### Development Workflow
1. Edit source files in `src/`
2. Run `npm run compile` or `npm run watch`
3. Launch via F5 in VS Code for debugging
4. Test changes in Extension Development Host

### Publishing Preparation
```bash
# Full production build with optimization
npm run vscode:prepublish
```

This runs the complete build pipeline including linting, compilation, and packaging.

### Package Dependencies
**Runtime Dependencies:** None (extension runs in VS Code context)

**Development Dependencies:**
- TypeScript compilation and type definitions
- Webpack for bundling
- ESLint for code quality
- VS Code testing framework
- Mocha for unit testing

## Key Implementation Details

### State Persistence
- Folders are persisted to VS Code's global/workspace storage
- Clipboard state is maintained in memory during session
- Temporary clipboard survives until explicitly cleared

### File Format Output
The extension generates markdown-formatted output with:
- Relative file paths as headers
- Language-specific code blocks
- Line number indicators for selections
- Separator lines between multiple files

### Performance Considerations
- Clipboard monitoring runs on 2-second intervals
- File watcher automatically cleans up deleted files
- Tree view updates are debounced to prevent excessive refreshes

### Error Handling
- Comprehensive logging system with debug/info/error levels
- Graceful degradation when files are deleted or moved
- User feedback through status bar indicators rather than popup notifications

## Common Development Tasks

### Adding a New Command
1. Add command definition to `package.json` in the `contributes.commands` section
2. Create a command handler in the appropriate file in `src/commands/`
3. Register the command in the corresponding registration function in `src/commands/`

### Debugging Tips
- Use the VS Code Extension Development Host for live debugging
- Check extension logs with `Copy Path with Code: Show Extension Logs` command
- Set breakpoints in the TypeScript code to debug specific functionality

This architecture enables efficient development of a feature-rich VS Code extension while maintaining clean separation of concerns and robust error handling.