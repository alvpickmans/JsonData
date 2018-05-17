# JsonData Change Log

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](http://keepachangelog.com/) and this project adheres to [Semantic Versioning](http://semver.org/).

## [Unreleased]

### Added
- Abstract `JsonOptionsBase` class inheriting from `NodeModel`, providing a reusable UI for nodes requiring `JsonOption` and `Nesting` options.
- Change log.
- Support for Dynamo Dictionaries (`DesignScript.Builtin.Dictionary`).

### Changed

- Transition to Dynamo 2.0.
- Previous Zero Touch nodes that have been replaced by `NodeModels` are set as `static` and hidden on Dynamo library.

### Removed
- `JsonArray` class.
- Dropdown node for `JsonOption` selector.
-  `jsonOption` and `nested` inputs on nodes requiring these options.