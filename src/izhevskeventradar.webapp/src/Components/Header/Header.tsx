import {
    AppBar,
    Box,
    Button,
    IconButton,
    Toolbar,
    Typography
} from '@mui/material';
import MenuIcon from '@mui/icons-material/Menu';
import { Link as RouterLink } from 'react-router-dom';

export default function Header() {


    return (
        <AppBar position="fixed" sx={{ zIndex: (theme) => theme.zIndex.drawer + 1 }}>
            <Toolbar>
                <Typography variant="h6" component="div">
                    Izhevsk Event Radar
                </Typography>

                <Box sx={{ flexGrow: 1 }} />

                <Box sx={{ display: { xs: 'none', md: 'block' } }}>
                    <Button color="inherit" component={RouterLink} to="/events">События</Button>
                    <Button color="inherit" component={RouterLink} to="/settings">Настройки</Button>
                </Box>

                <Box sx={{ display: { xs: 'block', md: 'none' } }}>
                    <IconButton
                        size="large"
                        edge="end"
                        color="inherit"
                        aria-label="menu"
                    >
                        <MenuIcon />
                    </IconButton>
                </Box>
            </Toolbar>
        </AppBar>
    );
}