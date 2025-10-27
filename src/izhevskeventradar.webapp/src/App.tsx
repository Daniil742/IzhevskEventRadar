import {
    Container,
    CssBaseline,
    Toolbar
} from '@mui/material';
import { AdapterDateFns } from '@mui/x-date-pickers/AdapterDateFns';
import { LocalizationProvider } from '@mui/x-date-pickers/LocalizationProvider';
import 'leaflet/dist/leaflet.css';
import { BrowserRouter, Navigate, Route, Routes } from 'react-router-dom';
import './App.css';
import EventsPage from './Components/Events/EventsPage';
import Header from './Components/Header/Header';
import SettingsPage from './Components/Settings/SettingsPage';

function App() {
    return (
        <LocalizationProvider dateAdapter={AdapterDateFns}>
            <BrowserRouter>
                <CssBaseline />
                <Header />
                <Toolbar />
                <Container
                    maxWidth='lg'
                    sx={{ mt: 3, mb: 3 }}
                >
                    <Routes>
                        <Route path="/events" element={<EventsPage />} />
                        <Route path="/settings" element={<SettingsPage />} />

                        <Route path="/" element={<Navigate to="/events" />} />
                    </Routes>
                </Container>
            </BrowserRouter>
        </LocalizationProvider>
    )
}

export default App
