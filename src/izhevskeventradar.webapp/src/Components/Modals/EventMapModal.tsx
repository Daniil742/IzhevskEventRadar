import CloseIcon from '@mui/icons-material/Close';
import {
    AppBar,
    Box,
    Button,
    CircularProgress,
    Dialog,
    DialogActions,
    DialogContent,
    IconButton,
    Toolbar,
    Typography
} from "@mui/material";
import { DatePicker } from '@mui/x-date-pickers/DatePicker';
import { useEffect, useState } from "react";
import type { IEvent } from '../../Interfaces/Events/IEvent';
import { fetchEventsByDate } from '../../Services/EventService';
import { MapContainer, Marker, Popup, TileLayer, useMap } from 'react-leaflet';
import { useMapEvents } from 'react-leaflet/hooks';
import '../../Utils/leafletIcon';

interface IEventMapModalProps {
    open: boolean;
    onClose: () => void;
    events: IEvent[];
    initialDate: Date | null;
}

const izhevskCenter: [number, number] = [56.852, 53.211];

function MapResizer({ isOpen }: { isOpen: boolean }) {
    const map = useMap();
    useEffect(() => {
        if (isOpen) {
            setTimeout(() => map.invalidateSize(), 100);
        }
    }, [isOpen, map]);
    return null;
}

export default function EventMapModal({ open, onClose, events: initialEvents, initialDate }: IEventMapModalProps) {

    const [date, setDate] = useState(initialDate);
    const [mapEvents, setMapEvents] = useState(initialEvents);
    const [loading, setLoading] = useState(false);

    useEffect(() => {
        setDate(initialDate);
    }, [initialDate]);

    useEffect(() => {
        if (date?.getTime() === initialDate?.getTime()) {
            setMapEvents(initialEvents);
            return;
        }

        const loadMapEvents = async () => {
            if (!date) {
                setMapEvents([]);
                return;
            }
            setLoading(true);
            try {
                const data = await fetchEventsByDate(date);
                setMapEvents(data);
            } catch (error) {
                console.error("Ошибка загрузки событий для карты:", error);
            } finally {
                setLoading(false);
            }
        };

        loadMapEvents();

    }, [date, initialDate, initialEvents]);

    return (
        <Dialog open={open} onClose={onClose} fullWidth maxWidth="lg">
            <AppBar sx={{ position: 'relative' }}>
                <Toolbar>
                    <Typography sx={{ flex: 1 }} variant="h6" component="div">
                        События на карте
                    </Typography>

                    <IconButton edge="end" color="inherit" onClick={onClose} aria-label="close">
                        <CloseIcon />
                    </IconButton>
                </Toolbar>
            </AppBar>

            <DialogContent dividers>
                <Box sx={{ display: 'flex', flexDirection: 'column', gap: 2 }}>
                    <DatePicker
                        label="Выберите дату"
                        value={date}
                        onChange={(newDate) => setDate(newDate)}
                    />

                    <Box
                        sx={{
                            height: '60vh',
                            backgroundColor: '#f0f0f0',
                            display: 'flex',
                            alignItems: 'center',
                            justifyContent: 'center',
                            borderRadius: 1
                        }}
                    >
                        {loading ? (
                            <Box sx={{ display: 'flex', alignItems: 'center', justifyContent: 'center', height: '100%' }}>
                                <CircularProgress />
                            </Box>
                        ) : (
                                <MapContainer
                                    center={izhevskCenter}
                                    zoom={13}
                                    style={{ height: '100%', width: '100%' }}
                                >
                                    <MapResizer isOpen={open} />

                                    <TileLayer
                                        attribution='&copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a> contributors'
                                        url="https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png"
                                    />

                                    {mapEvents.map((event: IEvent) => (
                                        <Marker
                                            key={event.id}
                                            position={[event.latitude, event.longitude]}
                                        >
                                            <Popup>
                                                <Typography variant="subtitle2">{event.title}</Typography>
                                                <Typography variant="body2">{event.location}</Typography>
                                            </Popup>
                                        </Marker>
                                    ))}
                                </MapContainer>
                        )}
                    </Box>
                </Box>
            </DialogContent>

            <DialogActions>
                <Button onClick={onClose}>Закрыть</Button>
            </DialogActions>
        </Dialog>
    );
}