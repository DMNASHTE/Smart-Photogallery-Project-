# Installation guide

## 1. Extract
Extract this ZIP to `C:\SmartPhotoGallery`.

## 2. Photo folder
Example:
`D:\PhotoArchive\Photos\2026\WomensDay\images\photo1.jpg`

Also supported:
`D:\PhotoArchive\Photos\2025\WomensDay\photo1.jpg`

The application reads photos from the configured root. It does not modify originals.

## 3. MySQL
Run `Database/schema.sql` in MySQL Workbench.
Then create a dedicated application account (choose your own password):
```sql
CREATE USER IF NOT EXISTS 'gallery_user'@'localhost' IDENTIFIED BY 'CHANGE_THIS_PASSWORD';
GRANT SELECT, INSERT, UPDATE, DELETE, CREATE, ALTER, INDEX
ON smart_photo_gallery.* TO 'gallery_user'@'localhost';
FLUSH PRIVILEGES;
```
Do not put your root password in the website configuration.

## 4. ASP.NET
Copy `WebApp/Web.config.example` to `WebApp/Web.config`.
Set `PhotoRoot`, `ImageUrlPrefix`, and the MySQL connection string.
`ImageUrlPrefix` must be an HTTP URL path served by IIS, not a Windows disk path.
For example, create an IIS virtual directory `/GalleryPhotos` mapped to the
same physical folder as `PhotoRoot`.
Open Visual Studio 2022 -> File -> Open -> Web Site -> choose `WebApp`.
Run `Default.aspx`.

The site is written in conservative C# syntax, but it is not a VS2012-tested
solution. The desktop target framework and MySQL Connector/NET version must be
verified on the office desktop.

## 5. Python environment
```cmd
cd /d C:\SmartPhotoGallery\AIService
python -m venv venv
venv\Scripts\activate
python -m pip install --upgrade pip
pip install -r requirements.txt
```
Copy the ONNX model files into `AIService\models`:
- face_detection_yunet_2023mar.onnx
- face_recognition_sface_2021dec.onnx

Get the models from official OpenCV Zoo repositories and retain licence files:
https://github.com/opencv/opencv_zoo/tree/main/models/face_detection_yunet
https://github.com/opencv/opencv_zoo/tree/main/models/face_recognition_sface

Start:
```cmd
python app.py
```
Health check: `http://127.0.0.1:5005/health`

## 6. Offline operation
Install Python packages, MySQL Connector/NET, browser assets, and model files
before disconnecting the internet. The supplied service binds to localhost.
Test the entire system with network disabled before deployment.

## 7. Limitations and safety
YuNet/SFace matching is probabilistic. Calibrate similarity thresholds against
your own photos. Use human review for candidate matches. Face embeddings are
sensitive biometric data; obtain appropriate consent and apply access controls,
retention rules and backups. Do not expose the local AI API to an untrusted network.
