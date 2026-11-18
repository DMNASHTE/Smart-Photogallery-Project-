Final Hardware and Software Requirements
Project: Offline AI-Based Face Search System
For this project, I recommend using a Python-based AI system with InsightFace, ONNX Runtime, OpenCV and MySQL, integrated with your existing ASP.NET Web Forms application.

Below is the complete setup, including what you need, why each component is important, and whether it is compulsory or optional.
1. Hardware requirements
Hardware is important because face detection and face recognition require processing power and memory, especially when processing thousands of photographs.
Hardware	Recommended specification	Why is it important?
RAM	16 GB recommended; 8 GB minimum	Allows Python, the AI model, database and other applications to run without excessive memory problems.
Processor	Intel Core i5 (10th Gen or newer) / AMD Ryzen 5 or better	Performs face detection, feature extraction and face matching. A better CPU improves processing speed.
Storage	512 GB SSD recommended; 256 GB minimum	Stores software, AI models, project files, database and temporary processing data. SSD improves loading speed.
GPU	Integrated graphics is sufficient to start	A dedicated GPU is not compulsory. CPU-based AI processing is possible.
Network	Gigabit LAN (if using separate machines)	Connects the Linux AI worker to the Windows server and photo storage.
Backup storage	External HDD (optional)	Provides an additional backup of important photos and database files.
UPS	Optional	Helps protect the server against unexpected power cuts.

My recommended hardware
16 GB RAM + Intel Core i5 / Ryzen 5 + 512 GB SSD + CPU-based processing.
Why do I recommend CPU instead of GPU initially?
Your project does not require a GPU to function. ONNX Runtime can execute the AI model using the CPU.
- CPU processing is easier to configure.
- You do not need to purchase an expensive graphics card.
- It is sufficient for development and testing with smaller photo collections.
- GPU acceleration can be added later if processing a large photo collection takes too long.
2. Software requirements
These are the technologies I recommend using for the project.
Software / technology	Required?	Why is it important?
Ubuntu Linux 22.04 or 24.04 LTS (64-bit)	For Linux AI worker	Provides the environment for running Python and AI processing.
Python 3.10	Yes	Main programming language for the AI processing component.
InsightFace	Yes, or equivalent licensed solution	Detects faces and extracts facial features for recognition.
ONNX Runtime	Yes	Executes the AI model locally using the CPU, without requiring cloud services.
OpenCV	Yes	Reads, loads, resizes and processes photographs.
MySQL	Yes	Stores face embeddings, photo paths, processing status and search information.
C# / ASP.NET Web Forms	Yes, existing application	Provides the website interface where users upload a reference photo and view matching photographs.
Visual Studio 2012	If maintaining existing application	Used to maintain and modify your existing ASP.NET project.
VS Code	Recommended	Makes Python development and debugging easier.
Git and GitHub	Recommended	Helps maintain code versions and document project progress.

3. Explanation of each important technology
1. Python – Main AI programming language
Compulsory
Python will handle the AI-related work.

It will:
- Read photographs from the existing photo storage.
- Send images to the face detection model.
- Extract facial features.
- Generate face embeddings.
- Compare a reference face with stored face embeddings.
Why important? Your existing ASP.NET application is built using an older .NET Framework. Python allows you to develop the AI component separately without rewriting the existing website.
2. InsightFace – Face detection and recognition
Compulsory, or use an alternative licensed solution






















8











InsightFace is the AI face-analysis toolkit.

It will:
- Detect faces in photographs.
- Identify the location of each face.
- Extract facial features.
- Generate numerical representations called face embeddings.
Why important? This is the main technology that enables your project to find photographs containing the same person.

Model note: buffalo_l is an InsightFace model pack that includes detection and recognition models. Its supplied pretrained models have non-commercial research-use restrictions. You must obtain an appropriate licence or select a model with suitable usage rights before deployment.
3. ONNX Runtime – Runs the AI model
Compulsory
ONNX Runtime executes the AI model.

It:
- Loads the model files.
- Performs AI calculations.
- Supports CPU processing.
- Can also support GPU acceleration when correctly configured.
Why important? It allows your face-recognition model to run locally without sending photographs to an online AI service.
4. OpenCV – Image processing
Compulsory
OpenCV is used to work with photographs.

It:
- Reads JPG, JPEG and PNG images.
- Resizes images when needed.
- Converts image formats.
- Prepares images for AI processing.
Why important? InsightFace needs properly loaded image data to detect and analyse faces.
5. MySQL – Database management
Compulsory
MySQL will store:
- Photo paths and identifiers.
- Face embeddings.
- Face detection information.
- Processing status.
- Search-related information.
Why important? Instead of analysing every photograph again whenever a user searches, the system can compare the reference face against previously generated embeddings.
6. ASP.NET Web Forms – Existing website
Compulsory for your existing application
Your existing website will remain the user interface.

It will:
- Allow users to upload a reference photograph.
- Send a face-search request.
- Receive matching photo information.
- Display matching photographs.
Why important? You do not need to rebuild your existing application. The AI functionality can be added as a separate component.
4. Recommended system architecture
Your project will work through the following process:
1. Existing ASP.NET Web Application

User uploads a reference photograph

2. MySQL Database

Stores search requests and face information

3. Python AI Worker

Processes photographs using InsightFace, ONNX Runtime and OpenCV

4. Face Embeddings

Numerical facial feature representations stored in MySQL

5. Face Matching

Compares the reference face with stored face embeddings

6. Search Results

Displays matching photographs in the existing website


5. What is compulsory and what is optional?
Component	Decision
Python	Required
Face-recognition model	Required
ONNX Runtime	Required for ONNX models
OpenCV	Required for this implementation
MySQL	Required for your proposed integration
Existing ASP.NET application	Required
8 GB RAM	Minimum recommended
16 GB RAM	Strongly recommended
NVIDIA GPU	Optional
Docker	Optional
External HDD	Optional
UPS	Optional

6. Operating system: Windows or Linux?
For your existing application, I recommend this arrangement:










Windows Server

Runs your existing ASP.NET Web Forms application and can continue hosting MySQL.













Ubuntu Linux

Runs the separate Python AI worker and performs face detection and recognition.













Local Area Network (LAN)

Connects the two systems and allows the AI worker to access authorised photo storage.



You do not need two computers if you do not have them. You can initially run the Python worker on the same Windows server. Linux is an option for the AI worker, not a compulsory requirement.
7. Offline working requirements
Your project must work without internet access during normal use.

For that, you must prepare:
1. Python and all required packages.
2. InsightFace or another selected face-recognition model.
3. ONNX model files.
4. ONNX Runtime CPU dependencies.
5. MySQL database and tables.
6. Local access to the photo storage.
7. All required project files.
After preparation, disconnect the internet and test the complete workflow. The application should not depend on cloud APIs or online model downloads.
8. Final configuration I recommend you follow
Your project setup
Recommended configuration
Hardware
- 16 GB RAM
- Intel Core i5 / AMD Ryzen 5 or better
- 512 GB SSD
- CPU processing
- Gigabit LAN if using separate machines

AI technologies
- Python 3.10
- InsightFace with a properly licensed model
- ONNX Runtime CPU
- OpenCV

Application and database
- Existing ASP.NET Web Forms
- C#
- MySQL

Development tools
- Ubuntu Linux for a separate AI worker, or Windows initially
- VS Code
- Git and GitHub

My final recommendation: Do not change your existing website or photo storage. Build the AI face-search functionality as a separate Python worker, store face embeddings and photo references in MySQL, and connect it to your existing ASP.NET application.

Start with CPU processing and a small sample of photographs. Test accuracy and speed before processing your entire photo collection. Also protect face embeddings and restrict access because they are sensitive biometric information.
