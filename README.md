# Dicom Receiver
A lightweight DICOM Receiver application built using WPF (.NET), Fellow Oak DICOM (fo-dicom), and SQLite. This project is designed to receive and store DICOM files from remote nodes over the network, extract essential metadata, and maintain a structured local database for medical imaging workflows.
✨ Features
✅ DICOM C-STORE SCP (Service Class Provider) support

✅ Handles incoming DICOM files and saves them locally

✅ Extracts and stores metadata (Patient, Study, Series, etc.) into a SQLite database

✅ Lightweight local PACS receiver simulation

✅ WPF-based user interface to manage nodes and server

✅ Support for DICOM C-ECHO (verification service)

✅ Extensible architecture for future modules (e.g., Worklist, Viewer)

🏗️ Tech Stack
.NET 9 / WPF

Fellow Oak DICOM (fo-dicom) – for DICOM networking and parsing

SQLite – for local metadata storage

MVVM pattern – for clean architecture

Asynchronous Networking – for responsive and stable communication

📂 DICOM Metadata Stored
Patient ID, Name, Age, Sex

Study UID, Date, Description, Modality

Series UID, Number, Body Part Examined

Accession Number, Referring Physician, etc.

📸 Usage
Add DICOM nodes with AE Title, IP, and Port.

Start the receiver server.

Remote systems can send DICOM files using C-STORE.

Files are saved locally; metadata is stored in SQLite.

Use the UI to manage studies and nodes.
