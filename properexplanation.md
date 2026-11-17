# Offline AI-Based Face Search System
### Project Research & Progress Report

## 1. Project Objective

To develop an offline face-search feature that can find photographs of a particular person from our existing photo collection using AI-based face recognition.

## 2. Technologies Studied

| Technology | Purpose |
|---|---|
| Python | AI processing |
| InsightFace | Face detection and recognition |
| Buffalo_L | Pre-trained AI model pack |
| ONNX | AI model format |
| ONNX Runtime | Runs AI models locally |
| MySQL | Stores face embeddings and photo references |
| ASP.NET Web Forms | Existing website and user interface |

## 3. Project Workflow

**Existing Photos (D:\Photos)**
  
↓  

**Python Background Worker**  
Scans existing photographs.

↓

**InsightFace (Buffalo_L)**  
Detects faces and extracts facial features.

↓

**ONNX Models + ONNX Runtime**  
Performs AI model calculations locally.

↓

**Face Embeddings**  
Generates numerical representations of faces.

↓

**MySQL Database**  
Stores embeddings and photo references.

↓

**Face Matching**  
Compares a reference person's face with stored faces.

↓

**Existing ASP.NET Website**  
Displays matching photographs.

## 4. Current Progress

**Research and planning completed:**
- Studied InsightFace and Buffalo_L.
- Understood face detection, recognition and embeddings.
- Studied ONNX and ONNX Runtime.
- Planned Python, MySQL and ASP.NET integration.
- Prepared the overall system workflow.

## 5. Next Implementation Steps

1. Set up Python environment and required libraries.
2. Prepare InsightFace model files.
3. Test face detection using sample images.
4. Generate and store face embeddings.
5. Implement face matching.
6. Integrate with the existing ASP.NET website.
7. Test the complete system offline.

## 6. Expected Outcome

An offline face-search feature integrated into the existing photo management system, allowing users to find photographs of a particular person without changing the original photo storage or replacing the existing website.

**Current Status:** Research & Architecture Planning Completed  
**Next Phase:** Practical Coding and Implementation

*Note: The pretrained Buffalo_L model's licence must be checked before production use.*
