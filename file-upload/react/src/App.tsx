import "./App.css";
import FileUpload from "./Components/FileUpload";

function App() {
    return (
        <div className="App">
            <header className="App-header">
                <div style={{ transform: "scale(2)" }}>
                    <FileUpload />
                </div>
            </header>
        </div>
    );
}

export default App;
