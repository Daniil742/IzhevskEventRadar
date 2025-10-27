import {
    Alert,
    Box,
    Button,
    CircularProgress,
    Divider,
    List,
    ListItem,
    ListItemText,
    Paper,
    Snackbar,
    Typography,
    type AlertColor
} from "@mui/material";
import { DatePicker } from "@mui/x-date-pickers/DatePicker";
import { useEffect, useState } from "react";
import { fetchEventsByDate } from "../../Services/EventService";
import EventMapModal from "../Modals/EventMapModal";
import type { IEvent } from '../../Interfaces/Events/IEvent';
import { ApiError } from "../../Utils/api";

export default function EventsPage() {

    const [events, setEvents] = useState<IEvent[]>([]);
    const [loading, setLoading] = useState(true);
    const [selectedDate, setSelectedDate] = useState<Date | null>(new Date());
    const [mapOpen, setMapOpen] = useState(false);

    const [snackOpen, setSnackOpen] = useState(false);
    const [snackSeverity, setSnackSeverity] = useState<AlertColor>('info'); // 'success' | 'error' | 'info' | 'warning'
    const [snackMessage, setSnackMessage] = useState('');

    useEffect(() => {
        if (!selectedDate) {
            setEvents([]);
            setLoading(false);
            return;
        }

        const controller = new AbortController();

        const loadEvents = async () => {
            setLoading(true);
            try {
                const data = await fetchEventsByDate(selectedDate, controller.signal);//fetchEventsByDate
                setEvents(data);
            } catch (err) {
                let message = 'Ошибка загрузки событий';

                if (err instanceof ApiError) {
                    message = err.message;
                }

                showSnack(message, 'error');
            } finally {
                setLoading(false);
            }
        };

        loadEvents();

        return () => { controller.abort(); };
    }, [selectedDate]);

    const showSnack = (message: string, severity: AlertColor = 'info') => {
        setSnackMessage(message);
        setSnackSeverity(severity);
        setSnackOpen(true);
    };

    const renderEventList = () => {
        if (loading) {
            return (
                <Box sx={{ display: 'flex', justifyContent: 'center', my: 4 }}>
                    <CircularProgress />
                </Box>
            );
        }

        if (events.length === 0) {
            return (
                <Typography sx={{ textAlign: 'center', my: 4 }}>
                    На выбранную дату событий не найдено.
                </Typography>
            );
        }

        return (
            <List sx={{ mt: 2 }}>
                {events.map((event: IEvent) => (
                    <ListItem key={event.id} divider>
                        <ListItemText
                            primary={event.title}
                            secondary={`${new Date(event.date).toLocaleTimeString('ru-RU', { hour: '2-digit', minute: '2-digit' })} - ${event.location}`}
                        />
                    </ListItem>
                ))}
            </List>
        );
    };

    return (
        <>
            <Paper elevation={3} sx={{ p: { xs: 2, md: 4 } }}>
                <Typography variant="h4" gutterBottom>
                    Календарь событий
                </Typography>

                <Box sx={{ display: 'flex', flexWrap: 'wrap', gap: 2, alignItems: 'center', mb: 2 }}>
                    <DatePicker
                        label="Выберите дату"
                        value={selectedDate}
                        onChange={(newDate) => setSelectedDate(newDate || new Date())}
                    />

                    <Button sx={{ flexGrow: 1}} />
                    <Button
                        variant="outlined"
                        onClick={() => setMapOpen(true)}
                    >
                        Посмотреть на карте
                    </Button>
                </Box>

                <Divider />

                {renderEventList()}
            </Paper>

            <EventMapModal
                open={mapOpen}
                onClose={() => setMapOpen(false)}
                events={events}
                initialDate={selectedDate}
            />

            <Snackbar
                open={snackOpen}
                autoHideDuration={4000}
                onClose={() => setSnackOpen(false)}
                anchorOrigin={{ vertical: 'bottom', horizontal: 'center' }}
            >
                <Alert onClose={() => setSnackOpen(false)} severity={snackSeverity} sx={{ width: '100%' }}>
                    {snackMessage}
                </Alert>
            </Snackbar>
        </>
    );
}