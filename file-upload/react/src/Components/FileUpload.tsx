import { useState } from "react";
import FileService from "../Services/FileService";

const FileUpload: React.FC = () => {
    const [file, setFile] = useState<File>();
    const [message, setMessage] = useState<string>("");
    const [progress, setProgress] = useState<number>(0);

    function selectFile(event: any) {
        setFile(event.target.files[0]);
    }

    const upload = () => {
        setProgress(0);
        if (!file) return;

        FileService.upload(file, (event: any) => {
            setProgress(Math.round((100 * event.loaded) / event.total));
        })
            .then((response) => {
                setMessage(response.data.message);
            })
            .catch((err) => {
                setProgress(0);

                if (
                    err.response &&
                    err.response.data &&
                    err.response.data.message
                ) {
                    setMessage(err.response.data.message);
                } else {
                    setMessage(`Upload error: ${err}`);
                }

                setFile(undefined);
            });
    };

    return (
        <>
            <div>
                <input type="file" onChange={selectFile} />
                <button disabled={!file} onClick={upload}>
                    Upload
                </button>
                <br />
                <br />
                <br />
            </div>

            {file && (
                <div>
                    <div>File name: {file?.name}</div>
                    <div>File size: {file?.size}</div>
                    <div>File size: {file?.type}</div>
                    <div>File lastModified: {file?.lastModified}</div>
                </div>
            )}

            {file && progress > 0 && (
                <div className="progress my-3">
                    <div
                        className="progress-bar progress-bar-info"
                        role="progressbar"
                        aria-valuenow={progress}
                        aria-valuemin={0}
                        aria-valuemax={100}
                        style={{ width: progress + "%" }}
                    >
                        Upload progress: {progress}%
                    </div>
                </div>
            )}

            {message && (
                <div className="alert alert-secondary mt-3" role="alert">
                    {message}
                </div>
            )}
        </>
    );
};

export default FileUpload;
