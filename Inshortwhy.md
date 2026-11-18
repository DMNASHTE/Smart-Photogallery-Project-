# Hardware and Software Requirements
## Project: Offline AI-Based Face Search System

### 1. Project Overview
The objective of this project is to develop an offline AI-based face-search system that allows users to find photographs containing a particular person using a reference photograph. The system will integrate with the existing ASP.NET Web Forms application without modifying the original photos or existing website structure.

### 2. Hardware Requirements

| Component | Recommended Specification | Importance |
|---|---|---|
| RAM | 16 GB (8 GB minimum) | Supports AI processing and multiple applications. |
| Processor | Intel Core i5 / AMD Ryzen 5 or better | Performs face detection and recognition calculations. |
| Storage | 512 GB SSD (256 GB minimum) | Stores software, models, database and project files. |
| GPU | Not compulsory | CPU can run the AI model; GPU can improve speed. |
| Network | Gigabit LAN | Connects separate Windows and Linux machines. |
| Backup | External HDD (optional) | Provides additional data backup. |

### 3. Software and Technology Requirements

| Technology | Purpose |
|---|---|
| Ubuntu Linux 64-bit | Operating system for the separate AI worker. |
| Python 3.10 | Main programming language for AI processing. |
| InsightFace | Detects faces and extracts facial features. |
| ONNX Runtime (CPU) | Runs the AI model locally without requiring a GPU. |
| OpenCV | Reads and processes photographs. |
| MySQL | Stores face embeddings, photo paths and processing information. |
| C# / ASP.NET Web Forms | Integrates face search into the existing website. |
| Visual Studio 2012 | Maintains the existing ASP.NET application. |
| VS Code and Git | Python development and source-code management. |

### 4. Working Architecture

**Existing ASP.NET Website**
↓  
**MySQL Database**
↓  
**Python AI Worker**
↓  
**InsightFace + ONNX Runtime + OpenCV**
↓  
**Face Detection and Face Embedding Generation**
↓  
**Face Matching**
↓  
**Display Matching Photographs in the Website**

### 5. Why These Technologies Are Important

- **Python:** Handles the complete AI-processing workflow.
- **InsightFace:** Detects faces and generates facial feature representations.
- **ONNX Runtime:** Executes the AI model locally, supporting offline operation.
- **OpenCV:** Loads and prepares photographs for AI processing.
- **MySQL:** Stores face embeddings and photo references, avoiding repeated processing.
- **ASP.NET:** Provides the existing website interface for uploading a reference photo and viewing search results.

### 6. Important Project Decisions

- Keep the existing ASP.NET Web Forms application on Windows.
- Run the Python AI worker on Windows or a separate Ubuntu Linux machine.
- Use CPU processing initially; a dedicated GPU is optional.
- Preserve all original photographs and existing database records.
- Prepare software dependencies and model files before offline deployment.
- Use a properly licensed face-recognition model. InsightFace's supplied `buffalo_l` pretrained models are restricted to non-commercial research use.

### 7. Final Recommended Configuration

**Hardware:** 16 GB RAM, Intel Core i5 / Ryzen 5, 512 GB SSD, CPU-based processing.

**Software:** Python, InsightFace (appropriately licensed model), ONNX Runtime, OpenCV, MySQL and existing ASP.NET Web Forms.

**Expected Outcome:** An offline face-search feature integrated into the existing photo management system, allowing users to search for photographs containing the same person using facial recognition.
