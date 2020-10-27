from ReliableCommunication import Client

class Robot:

    def __init__(self):
        self.conn = Client("127.0.0.1", 6000, autoReconnect=True)
        self.conn.connect()
        self.conn.add_onconnect_callback(self.__onConnStart)
        self.conn.add_onclose_callback(self.__onConnClose)
        self.conn.add_onmessage_callback(self.__onMessage)

        self.left_motor = Motor()
        self.right_motor = Motor()

    def left(self, speed=0.0):
        ''' Sets the left motor to rotate at a speed '''
        self.__activate_motor('left', speed)
        self.left_motor.__value = speed
    
    def right(self, speed=0.0):
        ''' Sets the right motor to rotate at a speed '''
        self.__activate_motor('right', speed)
        self.right_motor.__value = speed

    def set_motors(self, left_speed=0.0, right_speed=0.0):
        self.left_motor.value = left_speed
        self.right_motor.value = right_speed

    def __activate_motor(self, motorName, power):
        msg = {
            "type": "motor",
            "target": motorName,
            "power": power
        }
        self.conn.Send(msg)
    
    def stop(self):
        ''' Stops all motors '''
        msg = {
            "type": "stop"
        }
        self.conn.Send(msg)
        self.left_motor.__value = 0
        self.right_motor.__value = 0

    def __onMessage(self, message):
        pass
    
    def __onConnClose(self, message):
        pass

    def __onConnStart(self, message):
        pass

    def __update_motor_value(self, motor):
        if (motor is self.left_motor):
            self.left(motor.value)
        elif (motor is self.right_motor):
            self.right(motor.value)
        else:
            raise Exception("Unknown motor")


class Motor:

    def __init__(self, robot):
        self.__value = 0
        self.__robot = robot
    
    @property
    def value(self):
        return self.__value
    
    @value.setter
    def value(self, newValue):
        self.__value = newValue
        self.__robot.__update_motor_value(self)