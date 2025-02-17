import os
import time
import queue
import threading
import face_recognition
from io import BytesIO
from PIL import Image
from watchdog.observers import Observer
from watchdog.events import FileSystemEventHandler
from dotenv import load_dotenv

# Load environment variables
load_dotenv()

# Directories for input files and processed faces
input_folder = "input_images"
faces_folder = "faces"
os.makedirs(input_folder, exist_ok=True)
os.makedirs(faces_folder, exist_ok=True)

# Queue for processing files
file_queue = queue.Queue()

def process_image(file_path):
    """Detect and crop faces from an image file."""
    try:
        print(f"Processing file: {file_path}...")

        # Load the image
        image_np = face_recognition.load_image_file(file_path)
        image = Image.open(file_path)

        # Detect faces
        face_locations = face_recognition.face_locations(image_np)

        if face_locations:
            print(f"Detected {len(face_locations)} face(s) in {file_path}.")
            new_directory_name =  os.path.splitext(os.path.basename(file_path))[0]
            directory_path = os.path.join(faces_folder, new_directory_name)
            os.makedirs(directory_path, exist_ok=True)

            for face_idx, (top, right, bottom, left) in enumerate(face_locations, start=1):
                # Crop the face
                face_image = image.crop((left, top, right, bottom))
                face_file_path = os.path.join(directory_path, f"face_{face_idx}.jpg")
                face_image.save(face_file_path)
                print(f"Saved cropped face: {face_file_path}")
        else:
            print(f"No faces detected in {file_path}.")

    except Exception as e:
        print(f"Error processing {file_path}: {e}")

def worker():
    """Process files from the queue."""
    while True:
        file_path = file_queue.get()
        if file_path is None:
            break
        process_image(file_path)
        file_queue.task_done()

class DirectoryEventHandler(FileSystemEventHandler):
    """Handles events for the monitored directory."""
    def on_created(self, event):
        if event.is_directory:
            return
        print(f"File added to queue: {event.src_path}")
        file_queue.put(event.src_path)

def monitor_directory(path_to_watch):
    """Monitor a directory for new files and process them."""
    event_handler = DirectoryEventHandler()
    observer = Observer()
    observer.schedule(event_handler, path_to_watch, recursive=False)

    observer.start()
    print(f"Monitoring changes in: {path_to_watch}")

    try:
        while True:
            time.sleep(1)
    except KeyboardInterrupt:
        print("Stopping monitoring...")
        observer.stop()

    observer.join()

if __name__ == "__main__":
    # Start the worker thread
    worker_thread = threading.Thread(target=worker, daemon=True)
    worker_thread.start()

    # Monitor the directory
    monitor_directory(input_folder)

    # Wait for all tasks to complete
    file_queue.join()

    # Stop the worker thread
    file_queue.put(None)
    worker_thread.join()
