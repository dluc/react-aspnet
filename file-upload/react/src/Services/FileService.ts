import axios from "axios";

const apiHost = (window.document.location.protocol === "http:")
  ? "http://127.0.0.1:9000"
  : "https://127.0.0.1:9001";

const http = axios.create({
  baseURL: apiHost + "/api",
  // Default headers if unspecified
  headers: {
    "Content-type": "application/json",
  },
});

const upload = (file: File, onUploadProgress: any): Promise<any> => {
  let formData = new FormData();

  formData.append("file", file);

  return http.post("/upload", formData, {
    headers: {
      "Content-Type": "multipart/form-data",
    },
    onUploadProgress,
  });
};

const FileService = {
  upload
};

export default FileService;