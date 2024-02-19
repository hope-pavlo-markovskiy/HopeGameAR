class Tracker():
    def __init__(self):
        self.active = False
        self.frame = None

    def start(self):
        self.active = True

    def setFrame(self, frame):
        self.frame = frame

    def stop(self):
        self.active = False
