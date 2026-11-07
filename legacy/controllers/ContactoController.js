import { Contacto } from '../models/Contacto.js';

// Crear un contacto
const nuevoContacto = async (req, res, next) => {
    try {
        await Contacto.create({
            nombres: req.body.nombres,
            apellidos: req.body.apellidos,
            correo: req.body.correo,
            telefono: req.body.telefono,
            celular: req.body.celular,
            direccion: req.body.direccion
        });
        res.json({ mensaje: 'El contacto se agregó correctamente' });
    } catch (error) {
        console.log(error);
        next();
    }
}

//Obtiene todos los Contactos
const obtenerContactos = async (req, res, next) => {
    try {
        const contactos = await Contacto.findAll();
        res.json(contactos);
    } catch (error) {
        console.log(error);
        next();
    }
}

//Obtiene un Contacto en especifico por su ID
const obtenerContacto = async (req, res, next) => {
    try {
        let contactoId = req.params.id;
        const contacto = await Contacto.findOne({ where: { id: contactoId } });
        res.json(contacto)
    } catch (error) {
        console.log(error);
        next();
    }
}

//Actualiza un Contacto en especifico por su ID
const actualizarContacto = async (req, res, next) => {
    try {
        let contactoId = req.params.id;
        let nuevosDatos = req.body;
        const contacto = await Contacto.findOne({ where: { id: contactoId } })
            .then(persona => {
                persona.update(nuevosDatos)
            });
        res.json(contacto);
    } catch (error) {
        console.log(error);
        next();
    }
}

//Elimina Contacto por su ID
const eliminarContacto = async (req, res, next) => {
    try {
        let contactoId = req.params.id;
        await Contacto.destroy({ where: { id: contactoId } });
        res.json({ mensaje: 'El Contacto fue eliminado' });
    } catch (error) {
        console.log(error);
        next();
    }
}

export {
    nuevoContacto,
    obtenerContactos,
    obtenerContacto,
    actualizarContacto,
    eliminarContacto
}