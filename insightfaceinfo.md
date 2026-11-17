# Offline AI-Based Face Search System

## 1. Project Overview

This project aims to develop an Offline AI-Based Face Search System that can find photos of a particular person from an existing collection of photographs using face recognition technology.

The main goal is to integrate AI-based face search functionality into an existing ASP.NET Web Forms application without replacing the existing website, database or original photo storage.

The system is designed to perform AI processing locally without depending on cloud-based face recognition services.

## 2. Project Objectives

- Detect human faces from existing photographs.
- Extract facial features using AI models.
- Generate numerical face embeddings.
- Store face embeddings and photo references in MySQL.
- Register a person using a reference photograph.
- Compare faces and find matching photographs.
- Display matching photographs through the existing ASP.NET website.
- Maintain the existing photo storage and database structure.

## 3. Technologies Being Studied

| Technology | Purpose |
|---|---|
| Python | AI processing and background worker |
| InsightFace | Face detection, alignment and recognition |
| Buffalo_L | Pre-trained face analysis model pack |
| ONNX | Format for AI models |
| ONNX Runtime | Executes ONNX models locally |
| MySQL | Stores face embeddings and photo references |
| ASP.NET Web Forms | Existing website and user interface |
| C# | Existing application development |

## 4. Understanding InsightFace

InsightFace is an open-source face analysis toolkit.

Its main functionalities include:

- Face Detection: Locates faces in photographs.
- Face Alignment: Aligns detected faces using facial landmarks.
- Face Recognition: Extracts facial features for comparison.
- Face Embedding: Represents a face as a numerical vector.

These features will be used to build the person-based photo search functionality.

## 5. Understanding Buffalo_L

Buffalo_L is a pre-trained model pack provided by InsightFace.

It contains multiple models for face detection, face recognition and facial landmark detection.

Some components include:

- SCRFD-10GF – Face detection.
- ResNet50@WebFace600K – Face recognition and feature extraction.
- 2d106 – 2D facial landmark detection.
- 3d68 – 3D facial landmark detection.
- Gender and Age model – Age and gender estimation.

The model pack will be evaluated for the face detection and recognition requirements of this project.

**Note:** The published pretrained Buffalo_L models have non-commercial research restrictions. Appropriate licensing must be verified before production use.

## 6. Understanding ONNX and ONNX Runtime

ONNX stands for Open Neural Network Exchange.

It is a standard format for representing machine learning models.

ONNX Runtime is an inference engine used to execute ONNX models.

In this project:
- ONNX provides the model files.
- ONNX Runtime executes the models.
- InsightFace provides the face analysis functionality.

## 7. Proposed System Architecture

The planned processing flow is:

1. Existing photographs are stored in the local photo folder.
2. A Python background worker scans the photographs.
3. InsightFace detects faces in the photographs.
4. ONNX models process the detected faces.
5. Face embeddings are generated.
6. Embeddings and photo references are stored in MySQL.
7. A user provides a reference photograph of a person.
8. The system generates an embedding for the reference face.
9. The system compares it with stored embeddings.
10. Matching photo references are displayed through the existing ASP.NET website.

## 8. Project Flow

Existing Photo Collection  
↓  
Python Background Worker  
↓  
InsightFace Face Detection  
↓  
ONNX Models + ONNX Runtime  
↓  
Face Embedding Generation  
↓  
MySQL Database  
↓  
Person Registration and Face Matching  
↓  
Existing ASP.NET Website  
↓  
Display Matching Photographs

## 9. Current Research Progress

The following research and planning activities have been completed:

- Studied the InsightFace library.
- Studied face detection and face recognition concepts.
- Studied the Buffalo_L model pack.
- Understood ONNX and ONNX Runtime.
- Studied the concept of face embeddings.
- Planned Python and MySQL integration.
- Designed the proposed system architecture.
- Planned integration with the existing ASP.NET application.

## 10. Upcoming Implementation Tasks

- [ ] Set up the Python development environment.
- [ ] Install the required libraries.
- [ ] Prepare the InsightFace model files.
- [ ] Test face detection using sample photographs.
- [ ] Generate face embeddings.
- [ ] Design the required MySQL tables.
- [ ] Implement face matching.
- [ ] Create the Python background worker.
- [ ] Integrate the functionality with ASP.NET.
- [ ] Test the complete system in offline mode.

## 11. Expected Outcome

The expected outcome is an offline face-search feature integrated into the existing photo management application.

Users will be able to provide a person's reference photograph and find other photographs containing a matching face.

The existing photographs and website structure will be preserved.

---

**Project Status:** Research and Architecture Planning Completed. Practical Implementation is the Next Phase.
