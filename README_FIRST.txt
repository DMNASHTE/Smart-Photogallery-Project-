SMART PHOTO GALLERY - OFFLINE SOURCE PACKAGE

This is a source-code baseline for the requested project:
- ASP.NET Web Forms pages (.aspx + C# code-behind)
- MySQL schema for events, photos, keywords, people, embeddings and matches
- Python local face service using OpenCV YuNet + SFace ONNX models
- No cloud AI API is used by the included Python service

IMPORTANT:
This is not a certified production-ready build. It has not been compiled or
tested on your laptop or office desktop. You should compile and test it in
stages and send errors for correction. The ONNX model binaries are not bundled;
obtain them from the official OpenCV Zoo repositories and retain their licence
notices. Check exact model versions and permitted use before deployment.

The current web pages provide a basic folder-driven year/event/gallery flow.
Keyword persistence, database folder synchronization, person registration,
photo indexing, and end-to-end face search still require integration work.
Do not treat those features as complete just because their pages/schema exist.

First milestone:
1. Extract ZIP.
2. Read INSTALLATION_GUIDE.md.
3. Configure photo root and image URL mapping.
4. Run Home.aspx and test year -> event -> gallery browsing.
5. Then add MySQL connector and complete DB integration.
