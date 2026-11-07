import express from 'express';
import {
    nuevoContacto,
    obtenerContactos,
    obtenerContacto,
    actualizarContacto,
    eliminarContacto
} from '../controllers/ContactoController.js';

const router = express.Router();

router.post('/contactos', nuevoContacto);

router.get('/contactos', obtenerContactos);

router.get('/contactos/:id', obtenerContacto);

router.put('/contactos/:id', actualizarContacto);

router.delete('/contactos/:id', eliminarContacto);

export default router;